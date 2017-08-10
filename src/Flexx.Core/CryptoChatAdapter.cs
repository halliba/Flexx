using System;
using System.Text;
using System.Threading.Tasks;
using Flexx.Core.Api;

namespace Flexx.Core
{
    internal class CryptoChatAdapter : IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> PrivateMessageReceived;
        public event EventHandler<MessageReceivedEventArgs> PublicMessageReceived;
        public event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;

        private byte[] _keepAliveData;
        private bool _disposed;
        private readonly NetworkHandler _networkHandler;
        private readonly PersonalIdentity _identity;
        private readonly UserIdentity _publicIdentity;

        public CryptoChatAdapter(PersonalIdentity identity)
        {
            _identity = identity;
            _publicIdentity = new UserIdentity(_identity.Username, _identity.PublicKey);

            _networkHandler = new NetworkHandler();
            _networkHandler.PrivateDataIncoming += NetworkHandlerOnPrivateDataIncoming;
            _networkHandler.PublicDataIncoming += NetworkHandlerOnPublicDataIncoming;
        }

        #region incoming
        
        private async void NetworkHandlerOnPublicDataIncoming(object sender, DataIncomingEventArgs args)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var transport = await GetAndVerifyTransportAsync(args.Data);
                if (transport == null) return;

                var json = Encoding.Unicode.GetString(transport.Data);
                ChatPartner partner;
                switch (transport.Type)
                {
                    case ModelType.KeepAlive:
                        var keepAlive = await JsonUtils.DeserializeAsync<KeepAlive>(json);
                        if (!VerifySenderIntegrity(transport, keepAlive))
                            return;
                        partner = new ChatPartner(keepAlive.Sender, args.RemoteEndPoint.Address);
                        OnKeepAliveReceived(partner);
                        break;
                    case ModelType.Message:
                        var message = await JsonUtils.DeserializeAsync<Message>(json);
                        if (!VerifySenderIntegrity(transport, message))
                            return;
                        partner = new ChatPartner(message.Sender, args.RemoteEndPoint.Address);
                        OnKeepAliveReceived(partner);
                        OnPublicMessageReceived(partner, message);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception) when(_disposed)
            {
            }
        }

        private async void NetworkHandlerOnPrivateDataIncoming(object sender, DataIncomingEventArgs args)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var decrypted = CryptUtils.RsaDecryptWithPrivate(args.Data, _identity.KeyPair.Private);

                var transport = await GetAndVerifyTransportAsync(decrypted);
                if (transport == null) return;

                var json = Encoding.Unicode.GetString(transport.Data);
                var message = await JsonUtils.DeserializeAsync<Message>(json);
                if (!VerifySenderIntegrity(transport, message))
                    return;

                var partner = new ChatPartner(message.Sender, args.RemoteEndPoint.Address);
                OnKeepAliveReceived(partner);
                OnPrivateMessageReceived(partner, message);
            }
            catch (Exception) when(_disposed)
            {
            }
        }

        #endregion

        #region outgoing

        public async Task SendKeepAliveAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                if (_keepAliveData == null)
                {
                    var keepAlive = new KeepAlive
                    {
                        Sender = _publicIdentity
                    };
                    _keepAliveData = await CreateSignAndEncodeTransport(keepAlive, ModelType.KeepAlive);
                }

                await _networkHandler.SendPublicData(_keepAliveData);
            }
            catch (Exception) when(_disposed)
            {
            }
        }

        public async Task SendPublicMessageAsync(Message message)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var data = await CreateSignAndEncodeTransport(message, ModelType.Message);

                await _networkHandler.SendPublicData(data);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        public async Task SendPrivateMessageAsync(Message message, ChatPartner chatPartner)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var data = await CreateSignAndEncodeTransport(message, ModelType.Message);

                var publicKey = PemUtils.GetKeyFromPem(chatPartner.Identity.PublicKey);
                var encrypted = CryptUtils.RsaEncryptWithPublic(data, publicKey);

                await _networkHandler.SendPrivateData(encrypted, chatPartner.RemoteAdress);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        #region helper methods

        private static bool VerifySenderIntegrity(Transport transport, BaseModel model)
            => model.Sender.PublicKey == transport.PublicKey;

        private static async Task<Transport> GetAndVerifyTransportAsync(byte[] data)
        {
            var transport = await JsonUtils.DeserializeAsync<Transport>(Encoding.Unicode.GetString(data));

            var publicKey = PemUtils.GetKeyFromPem(transport.PublicKey);
            var isValid = SignUtils.Verify(transport.Data, transport.Signature, publicKey);

            return isValid ? transport : null;
        }
        
        private async Task<byte[]> CreateSignAndEncodeTransport(BaseModel model, ModelType type)
        {
            model.Sender = _publicIdentity;
            var transport = new Transport
            {
                Type = type,
                Data = Encoding.Unicode.GetBytes(await JsonUtils.SerializeAsync(model))
            };

            var signature = SignUtils.Sign(transport.Data, _identity.KeyPair.Private);
            transport.Signature = signature;
            transport.PublicKey = PemUtils.GetPemFromKey(_identity.KeyPair.Public);

            var json = await JsonUtils.SerializeAsync(transport);
            return Encoding.Unicode.GetBytes(json);
        }

        #endregion

        #region event incovators

        private void OnKeepAliveReceived(ChatPartner sender)
        {
            KeepAliveReceived?.Invoke(this, new KeepAliveReceivedEventArgs(sender));
        }

        private void OnPublicMessageReceived(ChatPartner sender, Message message)
        {
            PublicMessageReceived?.Invoke(this, new MessageReceivedEventArgs(sender, message));
        }

        private void OnPrivateMessageReceived(ChatPartner sender, Message message)
        {
            PrivateMessageReceived?.Invoke(this, new MessageReceivedEventArgs(sender, message));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            _disposed = true;
            _networkHandler.Dispose();
        }

        #endregion
    }
}