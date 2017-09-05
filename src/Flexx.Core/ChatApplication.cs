using System;
using System.Threading.Tasks;

namespace Flexx.Core
{
    public class ChatApplication : IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> PrivateMessageReceived;
        public event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;

        private readonly CryptoChatAdapter _cryptoAdapter;
        private bool _disposed;
        private int _keepAliveInterval = 5000;

        private bool SendKeepAlive { get; set; } = false;
        public PersonalIdentity Identity { get; }
        public ChatPartner Me { get; }

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
            Identity = identity;
            Me = new ChatPartner(identity);

            _cryptoAdapter = new CryptoChatAdapter(identity);
            _cryptoAdapter.PrivateMessageReceived +=
                (sender, args) => PrivateMessageReceived?.BeginInvoke(this, args, null, null);
            _cryptoAdapter.KeepAliveReceived +=
                (sender, args) => KeepAliveReceived?.BeginInvoke(this, args, null, null);

            SendKeepAliveAsync();
        }

        public PublicChatRoom EnterPublicChatRoom(string name, string preSharedKey) =>
            _cryptoAdapter.EnterPublicChatRoom(name, preSharedKey);

        public PublicChatRoom EnterPublicChatRoom() => _cryptoAdapter.EnterPublicChatRoom();

        public void LeavePublicChatRoom(PublicChatRoom chatRoom) => _cryptoAdapter.LeavePublicChatRoom(chatRoom);
        
        public async Task SendPrivateMessageAsync(string content, ChatPartner partner)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                await _cryptoAdapter.SendPrivateMessageAsync(content, partner);
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