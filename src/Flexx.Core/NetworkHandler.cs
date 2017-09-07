using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Flexx.Core.Protocol;
using Flexx.Core.Utils;

namespace Flexx.Core
{
    internal class NetworkHandler : IDisposable
    {
        private const ushort UdpPort = 34567;

        private static readonly byte[] MagicNumberBytes = Encoding.UTF8.GetBytes("FLEX");
        private static readonly int MagicNumber = BitConverter.ToInt32(MagicNumberBytes, 0);

        private readonly List<Guid> _receivedIds = new List<Guid>();

        public event EventHandler<PacketIncomingEventArgs> PacketIncoming;

        private readonly UdpClient _udpClient;
        private bool _disposed;

        public NetworkHandler()
        {
            _udpClient = new UdpClient(UdpPort);
            ListenUdpAsync();
        }

        #region incoming

        private async void ListenUdpAsync()
        {
            try
            {
                while (!_disposed)
                {
                    var result = await _udpClient.ReceiveAsync();
                    if (BitConverter.ToInt32(result.Buffer, 0) != MagicNumber)
                        continue;
                    HandleJsonAsync(result.Buffer, 4, result.Buffer.Length - 4);
                }
            }
            catch (Exception) when (_disposed)
            {
            }

            async void HandleJsonAsync(byte[] buffer, int offset, int length)
            {
                var json = Config.DefaultEncoding.GetString(buffer, offset, length);
                var packet = await JsonUtils.DeserializeAsync<Packet>(json);
                if (_receivedIds.Contains(packet.Id))
                    return;
                _receivedIds.Add(packet.Id);
                OnPacketIncoming(json, packet.Type);
            }
        }

        #endregion

        #region outgoing
        
        internal async Task SendPacketAsync(Packet packet)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            try
            {
                var json = await JsonUtils.SerializeAsync(packet);
                var jsonBytes = Config.DefaultEncoding.GetBytes(json);

                byte[] buffer;
                using (var memoryStream = new MemoryStream())
                {
                    await memoryStream.WriteAsync(MagicNumberBytes, 0, MagicNumberBytes.Length);
                    await memoryStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                    buffer = memoryStream.ToArray();
                }
                for (var i = 0; i < Config.PacketTrials; i++)
                {
                    await _udpClient.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, UdpPort));
                    await Task.Delay(Config.PacketTimeout);
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion
        
        #region event invocators

        private void OnPacketIncoming(string packetJson, ModelType packetType)
        {
            PacketIncoming?.BeginInvoke(this, new PacketIncomingEventArgs(packetJson, packetType), null, null);
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