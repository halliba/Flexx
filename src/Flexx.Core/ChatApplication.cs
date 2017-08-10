using System;
using System.Threading.Tasks;
using Flexx.Core.Api;

namespace Flexx.Core
{
    public class ChatApplication : IDisposable
    {
        private event EventHandler<MessageReceivedEventArgs> PrivateMessageReceived;
        private event EventHandler<MessageReceivedEventArgs> PublicMessageReceived;
        private event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;

        private readonly CryptoChatAdapter _cryptoAdapter;
        private bool _disposed;
        private int _keepAliveInterval = 5000;

        private bool SendKeepAlive { get; set; }

        public int KeepAliveInterval
        {
            get => _keepAliveInterval;
            set
            {
                if (_keepAliveInterval <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _keepAliveInterval = value;
            }
        }

        public ChatApplication(PersonalIdentity identity)
        {
            _cryptoAdapter = new CryptoChatAdapter(identity);
            _cryptoAdapter.PrivateMessageReceived +=
                (sender, args) => PrivateMessageReceived?.BeginInvoke(this, args, null, null);
            _cryptoAdapter.PublicMessageReceived +=
                (sender, args) => PublicMessageReceived?.BeginInvoke(this, args, null, null);
            _cryptoAdapter.KeepAliveReceived +=
                (sender, args) => KeepAliveReceived?.BeginInvoke(this, args, null, null);

            SendKeepAliveAsync();
        }

        public async Task SendPublicMessageAsync(Message message)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                await _cryptoAdapter.SendPublicMessageAsync(message);
            }
            catch (Exception) when (_disposed)
            {

            }
        }

        public async Task SendPrivateMessageAsync(Message message, ChatPartner partner)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                await _cryptoAdapter.SendPrivateMessageAsync(message, partner);
            }
            catch (Exception) when (_disposed)
            {
                
            }
        }

        private async void SendKeepAliveAsync()
        {
            try
            {
                while (true)
                {
                    if (SendKeepAlive)
                    {
                        await _cryptoAdapter.SendKeepAliveAsync();
                    }
                    await Task.Delay(KeepAliveInterval);
                }
            }
            catch (Exception) when(_disposed)
            {
            }
        }

        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            _disposed = true;
            _cryptoAdapter.Dispose();
        }

        #endregion
    }
}