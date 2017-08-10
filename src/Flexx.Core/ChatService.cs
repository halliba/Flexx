using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Flexx.Core.Api;
using Newtonsoft.Json;

namespace Flexx.Core
{
    public class ChatService : IDisposable
    {
        private readonly ConcurrentDictionary<UserIdentity, DateTime> _users = new ConcurrentDictionary<UserIdentity, DateTime>();

        private readonly ChatServiceOptions _options = new ChatServiceOptions
        {
            Username = "User" + new Random().Next(0, 1000)
        };

        public UserIdentity MyIdentity { get; }

        public event EventHandler<Message> MessageReceived;

        public event EventHandler<UserIdentity> UserEntered;

        public event EventHandler<UserIdentity> UserLeft;
        
        private UdpClient _udpClient;

        private readonly MessageCryptoProvider _cryptoProvider;
        
        public ChatService(Action<ChatServiceOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            configure(_options);

            _cryptoProvider = MessageCryptoProvider.Generate();
            _udpClient = new UdpClient(_options.Port);
            MyIdentity = new UserIdentity(_options.Username, PemUtils.GetPemFromKey(_cryptoProvider.KeyPair.Public));
        }

        public void ImportPrivateKey(string pemFormatted)
        {
            _cryptoProvider.KeyPair = PemUtils.GetKeyPairFromPem(pemFormatted);
        }

        public string ExportPrivateKey() => PemUtils.GetPemFromKeyPair(_cryptoProvider.KeyPair);

        public void BeginAsync()
        {
            ListenAsync();
            SendKeepAliveAsync();
            ProcessUsersAsync();
        }

        private async void ListenAsync()
        {
            while (_udpClient != null)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    var bytes = result.Buffer;
                    var text = Encoding.Unicode.GetString(bytes);
                    var transport = JsonConvert.DeserializeObject<Transport>(text);
                    
                    if (transport.Encrypted && !_cryptoProvider.DecryptTransport(transport))
                        continue;

                    switch (transport.Type)
                    {
                        case ModelType.KeepAlive:
                            if (!_cryptoProvider.ValidateTransport(transport, out KeepAlive keepAlive))
                                continue;
                            HandleKeepAlive(keepAlive.Sender);
                            break;
                        case ModelType.Message:
                            if (!_cryptoProvider.ValidateTransport(transport, out Message message))
                                continue;
                            HandleKeepAlive(message.Sender);
                            OnMessageReceived(message);
                            break;
                        default:
                            continue;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public async void SendMessageAsync(Message message)
        {
            await SendAsync(ModelType.Message, message);
        }

        private async Task SendAsync(ModelType type, BaseModel data)
        {
            data.Sender = MyIdentity;
            var transport = new Transport
            {
                Type = type,
                Data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(data))
            };
            _cryptoProvider.SignTransport(transport);

            var json = JsonConvert.SerializeObject(transport);
            var bytes = Encoding.Unicode.GetBytes(json);

            try
            {
                await _udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, _options.Port));
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private async void SendKeepAliveAsync()
        {
            var keepAlive = new Transport
            {
                Type =  ModelType.KeepAlive,
                Data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new KeepAlive
                {
                    Sender = MyIdentity
                }))
            };
            _cryptoProvider.SignTransport(keepAlive);
            var json = JsonConvert.SerializeObject(keepAlive);
            var bytes = Encoding.Unicode.GetBytes(json);

            try
            {
                while (_udpClient != null)
                {
                    await _udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, _options.Port));
                    await Task.Delay(5000);
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private async void ProcessUsersAsync()
        {
            var deadTime = TimeSpan.FromSeconds(10);
            while (_udpClient != null)
            {
                var breakPoint = DateTime.Now - deadTime;
                var toRemove = _users.Where(u => u.Value < breakPoint).Select(u => u.Key).ToArray();
                foreach (var userIdentity in toRemove)
                {
                    ((IDictionary<UserIdentity, DateTime>)_users).Remove(userIdentity);
                    OnUserLeft(userIdentity);
                }
                await Task.Delay(2000);
            }
        }

        private void HandleKeepAlive(UserIdentity identity)
        {
            if (_users.ContainsKey(identity))
            {
                _users[identity] = DateTime.Now;
            }
            else
            {
                _users.TryAdd(identity, DateTime.Now);
                OnUserEntered(identity);
            }
        }

        #region event incocators

        private void OnMessageReceived(Message message)
        {
            MessageReceived?.BeginInvoke(this, message, null, null);
        }

        private void OnUserEntered(UserIdentity user)
        {
            UserEntered?.Invoke(this, user);
        }

        private void OnUserLeft(UserIdentity user)
        {
            UserLeft?.Invoke(this, user);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _udpClient?.Dispose();
            _udpClient = null;
        }

        #endregion
    }
}