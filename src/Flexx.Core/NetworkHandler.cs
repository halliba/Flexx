using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Flexx.Core
{
    internal class NetworkHandler : IDisposable
    {
        private const ushort UdpPort = 57789;
        private const ushort TcpPort = 57790;

        public event EventHandler<DataIncomingEventArgs> PublicDataIncoming;
        public event EventHandler<DataIncomingEventArgs> PrivateDataIncoming;
        
        private readonly TcpListener _tcpListener;
        private readonly UdpClient _udpClient;
        private bool _disposed;

        public NetworkHandler()
        {
            _udpClient = new UdpClient(UdpPort);
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, TcpPort));
            ListenUdpAsync();
            ListenTcpAsync();
        }

        #region incoming

        private async void ListenUdpAsync()
        {
            try
            {
                while (!_disposed)
                {
                    var result = await _udpClient.ReceiveAsync();
                    OnPublicDataIncoming(result.Buffer, result.RemoteEndPoint);
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        private async void ListenTcpAsync()
        {
            try
            {
                while (!_disposed)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    HandleClientAsync(client);
                }

                async void HandleClientAsync(TcpClient client)
                {
                    await Task.Run(async () =>
                    {
                        byte[] result;
                        using (var stream = client.GetStream())
                        {
                            var buffer = new byte[4];
                            await stream.ReadAsync(buffer, 0, buffer.Length);
                            var length = BitConverter.ToInt32(buffer, 0);
                            using (var memoryStream = new MemoryStream())
                            {
                                await CopyStreamAsync(stream, memoryStream, 1024, length);
                                result = memoryStream.ToArray();
                            }
                        }
                        OnPrivateDataIncoming(result, (IPEndPoint)client.Client.RemoteEndPoint);
                    });
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        #region outgoing

        internal async Task SendPublicData(byte[] data)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            try
            {
                await _udpClient.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Broadcast, UdpPort));
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        internal async Task SendPrivateData(byte[] data, IPAddress targetAdress)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(targetAdress, TcpPort);
                    var stream = tcpClient.GetStream();
                    await stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4);
                    using (var memoryStream = new MemoryStream(data))
                    {
                        await CopyStreamAsync(memoryStream, stream, 1024, data.Length);
                    }
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        private static async Task CopyStreamAsync(Stream source, Stream destination, int bufferSize, long sourceBytes)
        {
            var processed = 0L;
            while (processed < sourceBytes)
            {
                var remaining = sourceBytes - processed;
                var length = (int)(remaining < bufferSize ? remaining : bufferSize);
                var data = new byte[length];
                var bytesRead = await source.ReadAsync(data, 0, length);
                await destination.WriteAsync(data, 0, bytesRead);
                processed += bytesRead;
            }
        }

        #region event invocators

        private void OnPublicDataIncoming(byte[] data, IPEndPoint remoteEndPoint)
        {
            PublicDataIncoming?.BeginInvoke(this, new DataIncomingEventArgs(data, remoteEndPoint), null, null);
        }

        private void OnPrivateDataIncoming(byte[] data, IPEndPoint remoteEndPoint)
        {
            PrivateDataIncoming?.BeginInvoke(this, new DataIncomingEventArgs(data, remoteEndPoint), null, null);
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            _disposed = true;
            _udpClient?.Dispose();
        }

        #endregion
    }
}