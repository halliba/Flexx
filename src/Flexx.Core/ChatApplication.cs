using System;
using System.Threading.Tasks;

namespace Flexx.Core
{
    public class ChatApplication : IDisposable
    {
        public event EventHandler<InviteReceivedEventArgs> InviteReceived;
        public event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;

        private readonly CryptoChatAdapter _cryptoAdapter;
        private bool _disposed;
        private int _keepAliveInterval = 5000;

        public bool SendKeepAlive { get; set; } = false;
        public PersonalIdentity Identity { get; }
        public UserIdentity Me { get; }

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
            Me = identity;

            _cryptoAdapter = new CryptoChatAdapter(identity);
            _cryptoAdapter.InviteReceived +=
                (sender, args) => InviteReceived?.BeginInvoke(this, args, null, null);
            _cryptoAdapter.KeepAliveReceived +=
                (sender, args) => KeepAliveReceived?.BeginInvoke(this, args, null, null);

            SendKeepAliveAsync();
        }

        public PublicChatRoom EnterPublicChatRoom(string name, string preSharedKey) =>
            _cryptoAdapter.EnterPublicChatRoom(name, preSharedKey);
        
        public PublicChatRoom EnterPublicChatRoom(string name, byte[] preSharedKey) =>
            _cryptoAdapter.EnterPublicChatRoom(name, preSharedKey);

        public PublicChatRoom EnterPublicChatRoom() => _cryptoAdapter.EnterPublicChatRoom();
        
        public async Task SendInviteAsync(string name, string password, UserIdentity chatPartner)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var preSharedKey = PublicChatRoom.GeneratePreSharedKey(password);
                await _cryptoAdapter.SendInviteAsync(name, preSharedKey, chatPartner);
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