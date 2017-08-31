using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexx.Core.Api;

namespace Flexx.Core
{
    internal class CryptoChatAdapter : IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> PrivateMessageReceived;
        public event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;

        private byte[] _keepAliveData;
        private bool _disposed;
        private readonly NetworkHandler _networkHandler;
        private readonly PersonalIdentity _personalIdentity;
        private readonly UserIdentity _publicIdentity;

        private readonly List<PublicChatRoom> _publicRooms = new List<PublicChatRoom>();

        internal CryptoChatAdapter(PersonalIdentity identity)
        {
            _personalIdentity = identity;
            _publicIdentity = new UserIdentity(_personalIdentity.Username, _personalIdentity.PublicKey);

            _networkHandler = new NetworkHandler();
            _networkHandler.PrivateDataIncoming += NetworkHandlerOnPrivateDataIncoming;
            _networkHandler.PublicDataIncoming += NetworkHandlerOnPublicDataIncoming;
        }

        internal PublicChatRoom EnterPublicChatRoom(string name, string preSharedKey)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (preSharedKey == null) throw new ArgumentNullException(nameof(preSharedKey));
            var chatRoom = new PublicChatRoom(this, name, preSharedKey);
            _publicRooms.Add(chatRoom);
            return chatRoom;
        }

        internal PublicChatRoom EnterPublicChatRoom() => EnterPublicChatRoom("Default", "Default");

        internal void LeavePublicChatRoom(PublicChatRoom chatRoom)
        {
            _publicRooms.Remove(chatRoom);
        }

        #region incoming

        private async void NetworkHandlerOnPublicDataIncoming(object sender, DataIncomingEventArgs args)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                if (args.Data.Length < 4)
                    return;

                var type = (ModelType) BitConverter.ToInt32(args.Data, 0);

                ChatPartner partner;
                Transport transport;
                string json;
                switch (type)
                {
                    case ModelType.KeepAlive:
                        transport = await GetAndVerifyTransportAsync(args.Data, 4, args.Data.Length - 4);
                        if (transport == null) return;
                        json = Encoding.Unicode.GetString(transport.Data);

                        var keepAlive = await JsonUtils.DeserializeAsync<KeepAlive>(json);
                        if (!VerifySenderIntegrity(transport, keepAlive))
                            return;

                        partner = new ChatPartner(keepAlive.Sender, args.RemoteEndPoint.Address);
                        OnKeepAliveReceived(partner);
                        break;
                    case ModelType.Message:
                        var identifier = new byte[64];
                        Array.Copy(args.Data, 4, identifier, 0, 64);
                        var chatRoom = _publicRooms.FirstOrDefault(r => r.Identifier.SequenceEqual(identifier));
                        if (chatRoom == null)
                            return;

                        var encrypted = new byte[args.Data.Length - (4 + 64)];
                        Buffer.BlockCopy(args.Data, 4 + 64, encrypted, 0, encrypted.Length);
                        var decrypted = CryptUtils.AesDecryptBytes(encrypted, chatRoom.PreSharedKey);
                        
                        transport = await GetAndVerifyTransportAsync(decrypted, 0, decrypted.Length);
                        if (transport == null) return;
                        json = Encoding.Unicode.GetString(transport.Data);

                        var message = await JsonUtils.DeserializeAsync<Message>(json);
                        if (!VerifySenderIntegrity(transport, message))
                            return;

                        partner = new ChatPartner(message.Sender, args.RemoteEndPoint.Address);
                        OnKeepAliveReceived(partner);
                        chatRoom.OnPublicMessageReceived(new MessageReceivedEventArgs(partner, message));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        private async void NetworkHandlerOnPrivateDataIncoming(object sender, DataIncomingEventArgs args)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var encrypedAesLength = BitConverter.ToInt32(args.Data, 0);
                var aesKey =
                    CryptUtils.RsaDecryptWithPrivate(args.Data, 4, encrypedAesLength, _personalIdentity.KeyPair.Private);
                Console.WriteLine(BitConverter.ToString(aesKey));

                var encrypted = new byte[args.Data.Length - (4 + encrypedAesLength)];
                Buffer.BlockCopy(args.Data, 4 + encrypedAesLength, encrypted, 0, encrypted.Length);
                var decrypted = CryptUtils.AesDecryptBytes(encrypted, aesKey);

                var transport = await GetAndVerifyTransportAsync(decrypted, 0, 0);
                if (transport == null) return;

                var json = Encoding.Unicode.GetString(transport.Data);
                var message = await JsonUtils.DeserializeAsync<Message>(json);
                if (!VerifySenderIntegrity(transport, message))
                    return;

                var partner = new ChatPartner(message.Sender, args.RemoteEndPoint.Address);
                OnKeepAliveReceived(partner);
                OnPrivateMessageReceived(partner, message);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        #region outgoing

        internal async Task SendKeepAliveAsync()
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

                    var typeIdentifier = BitConverter.GetBytes((int) ModelType.KeepAlive);
                    var keepAliveData = await CreateSignAndEncodeTransportAsync(keepAlive);

                    _keepAliveData = new byte[typeIdentifier.Length + keepAliveData.Length];
                    Buffer.BlockCopy(typeIdentifier, 0, _keepAliveData, 0, typeIdentifier.Length);
                    Buffer.BlockCopy(keepAliveData, 0, _keepAliveData, typeIdentifier.Length, keepAliveData.Length);
                }
                
                await _networkHandler.SendPublicData(_keepAliveData);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        internal async Task SendPublicMessageAsync(PublicChatRoom chatRoom, string content)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            if(!_publicRooms.Contains(chatRoom))
                throw new NotSupportedException("Enter ChatRoom first.");

            try
            {
                var message = new Message
                {
                    Content = content,
                    Sender = _publicIdentity
                };
                var transport = await CreateSignAndEncodeTransportAsync(message);
                var encrypted = CryptUtils.AesEncryptByteArray(transport, chatRoom.PreSharedKey);

                byte[] data;
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(BitConverter.GetBytes((int) ModelType.Message), 0, 4);

                    memoryStream.Write(chatRoom.Identifier, 0, chatRoom.Identifier.Length);

                    memoryStream.Write(encrypted, 0, encrypted.Length);

                    data = memoryStream.ToArray();
                }

                await _networkHandler.SendPublicData(data);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        internal async Task SendPrivateMessageAsync(Message message, ChatPartner chatPartner)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var data = await CreateSignAndEncodeTransportAsync(message);

                var publicKey = PemUtils.GetKeyFromPem(chatPartner.Identity.PublicKey);
                var aesKey = CryptUtils.GenrateAesKey();
                Console.WriteLine(BitConverter.ToString(aesKey));

                var encryptedAesKey = CryptUtils.RsaEncryptWithPublic(aesKey, publicKey);
                var encryptedData = CryptUtils.AesEncryptByteArray(data, aesKey);

                byte[] result;
                using (var memoryStream = new MemoryStream())
                {
                    await memoryStream.WriteAsync(BitConverter.GetBytes(encryptedAesKey.Length), 0, 4);
                    await memoryStream.WriteAsync(encryptedAesKey, 0, encryptedAesKey.Length);
                    await memoryStream.WriteAsync(encryptedData, 0, encryptedData.Length);
                    result = memoryStream.ToArray();
                }

                await _networkHandler.SendPrivateData(result, chatPartner.RemoteAdress);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        #region helper methods

        private static bool VerifySenderIntegrity(Transport transport, BaseModel model)
            => model.Sender.PublicKey == transport.PublicKey;

        private static async Task<Transport> GetAndVerifyTransportAsync(byte[] data, int index, int count)
        {
            var json = Encoding.Unicode.GetString(data, index, count);
            var transport = await JsonUtils.DeserializeAsync<Transport>(json);

            var publicKey = PemUtils.GetKeyFromPem(transport.PublicKey);
            var isValid = SignUtils.Verify(transport.Data, transport.Signature, publicKey);

            return isValid ? transport : null;
        }

        private async Task<byte[]> CreateSignAndEncodeTransportAsync(BaseModel model)
        {
            model.Sender = _publicIdentity;
            var transport = new Transport
            {
                Data = Encoding.Unicode.GetBytes(await JsonUtils.SerializeAsync(model))
            };
            
            var signature = SignUtils.Sign(transport.Data, _personalIdentity.KeyPair.Private);
            transport.Signature = signature;
            transport.PublicKey = PemUtils.GetPemFromKey(_personalIdentity.KeyPair.Public);

            var json = await JsonUtils.SerializeAsync(transport);
            var bytes = Encoding.Unicode.GetBytes(json);
            return bytes;
        }

        #endregion

        #region event incovators

        private void OnKeepAliveReceived(ChatPartner sender)
        {
            KeepAliveReceived?.Invoke(this, new KeepAliveReceivedEventArgs(sender));
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