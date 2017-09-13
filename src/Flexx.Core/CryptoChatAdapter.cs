using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Flexx.Core.Protocol;
using Flexx.Core.Utils;
using Org.BouncyCastle.Crypto;

namespace Flexx.Core
{
    internal class CryptoChatAdapter : IDisposable
    {
        public event EventHandler<InviteReceivedEventArgs> InviteReceived;
        public event EventHandler<KeepAliveReceivedEventArgs> KeepAliveReceived;
        
        private bool _disposed;
        private readonly NetworkHandler _networkHandler;
        private readonly PersonalIdentity _personalIdentity;
        private readonly UserIdentity _publicIdentity;
        
        private readonly List<PublicChatRoom> _publicRooms = new List<PublicChatRoom>();

        internal CryptoChatAdapter(PersonalIdentity identity)
        {
            _personalIdentity = identity;
            _publicIdentity = new UserIdentity(_personalIdentity.Name, _personalIdentity.PublicKey);

            _networkHandler = new NetworkHandler();
            _networkHandler.PacketIncoming += NetworkHandlerOnPacketIncoming;
        }

        internal PublicChatRoom EnterPublicChatRoom(string name, string preSharedKey)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (preSharedKey == null) throw new ArgumentNullException(nameof(preSharedKey));
            var chatRoom = new PublicChatRoom(this, name, preSharedKey);
            _publicRooms.Add(chatRoom);
            return chatRoom;
        }

        internal PublicChatRoom EnterPublicChatRoom(string name, byte[] preSharedKey)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (preSharedKey == null) throw new ArgumentNullException(nameof(preSharedKey));
            var chatRoom = new PublicChatRoom(this, name, preSharedKey);
            _publicRooms.Add(chatRoom);
            return chatRoom;
        }

        internal PublicChatRoom EnterPublicChatRoom() => EnterPublicChatRoom(Config.DefaultChatRoom.Name, Config.DefaultChatRoom.Password);

        internal void LeavePublicChatRoom(PublicChatRoom chatRoom)
        {
            _publicRooms.Remove(chatRoom);
        }

        #region incoming

        private async void NetworkHandlerOnPacketIncoming(object sender, PacketIncomingEventArgs args)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var packet = await JsonUtils.DeserializeAsync<Packet>(args.PacketJson);
                if (packet == null)
                    return;
                
                switch (packet.Type)
                {
                    case ModelType.KeepAlive:
                        HandleIncomingKeepAliveAsync(args.PacketJson);
                        break;
                    case ModelType.Message:
                        HandleIncomingPublicMessageAsync(args.PacketJson);
                        break;
                    case ModelType.Invite:
                        HandleIncomingInviteAsync(args.PacketJson);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        private async void HandleIncomingInviteAsync(string json)
        {
            var packet = await JsonUtils.DeserializeAsync<InvitePacket>(json);
            if (packet?.Content == null || packet.AesKey == null)
                return;

            Invite invite;
            try
            {
                var aesKey = CryptUtils.RsaDecryptWithPrivate(packet.AesKey, 0, packet.AesKey.Length,
                    _personalIdentity.KeyPair.Private);
                var decrypted = CryptUtils.AesDecryptBytes(packet.Content, aesKey);
                var signedDataJson = Config.DefaultEncoding.GetString(decrypted);

                invite = await GetAndVerifySignedDataAsync<Invite>(signedDataJson);
                if (invite?.Name == null
                    || invite.PreSharedKey == null
                    || invite.Sender?.Name == null
                    || invite.Sender.PublicKey == null) return;
            }
            catch (CryptographicException)
            {
                return;
            }
            catch (InvalidCipherTextException)
            {
                return;
            }
            
            OnKeepAliveReceived(invite.Sender);
            OnInviteReceived(invite.Name, invite.PreSharedKey, invite.Sender);
        }

        private async void HandleIncomingKeepAliveAsync(string json)
        {
            var packet = await JsonUtils.DeserializeAsync<KeepAliveMessagePacket>(json);
            if (packet?.Content == null)
                return;

            var packetContent = Config.DefaultEncoding.GetString(packet.Content);
            var keepAlive = await GetAndVerifySignedDataAsync<KeepAlive>(packetContent);
            if (keepAlive?.Sender == null)
                return;
            
            OnKeepAliveReceived(keepAlive.Sender);
        }

        private async void HandleIncomingPublicMessageAsync(string json)
        {
            var packet = await JsonUtils.DeserializeAsync<PublicMessagePacket>(json);
            if (packet == null)
                return;

            var chatRoom = _publicRooms.FirstOrDefault(r => r.Equals(packet.ChatRoom));
            if (chatRoom == null)
                return;
            
            var decrypted = CryptUtils.AesDecryptBytes(packet.Content, chatRoom.PreSharedKey);
            var signedJson = Config.DefaultEncoding.GetString(decrypted);

            var message = await GetAndVerifySignedDataAsync<Message>(signedJson);
            if (message?.Sender == null) return;
            
            OnKeepAliveReceived(message.Sender);
            chatRoom.OnPublicMessageReceived(new MessageReceivedEventArgs(message.Sender, message));
        }

        #endregion

        #region outgoing

        internal async Task SendKeepAliveAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            try
            {
                var keepAlive = new KeepAlive
                {
                    Sender = _publicIdentity
                };
                
                var keepAliveData = await CreateSignAndEncodeDataAsync(keepAlive);
                var packet = new KeepAliveMessagePacket(keepAliveData);
                
                await _networkHandler.SendPacketAsync(packet);
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
                    Sender = _publicIdentity,
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };
                var signedData = await CreateSignAndEncodeDataAsync(message);
                var encrypted = CryptUtils.AesEncryptByteArray(signedData, chatRoom.PreSharedKey);

                var packet = new PublicMessagePacket(encrypted, chatRoom.GetEncryptedIdentifier());

                await _networkHandler.SendPacketAsync(packet);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        internal async Task SendInviteAsync(string name, byte[] preSharedKey, UserIdentity receiver)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            try
            {
                var invite = new Invite
                {
                    Name = name,
                    PreSharedKey = preSharedKey,
                    Sender = _publicIdentity
                };
                var signedData = await CreateSignAndEncodeDataAsync(invite);

                var publicKey = PemUtils.GetKeyFromPem(receiver.PublicKey);
                var aesKey = CryptUtils.GenrateAesKey();

                var encryptedAesKey = CryptUtils.RsaEncryptWithPublic(aesKey, publicKey);
                var encryptedData = CryptUtils.AesEncryptByteArray(signedData, aesKey);

                var packet = new InvitePacket(encryptedData, encryptedAesKey);

                await _networkHandler.SendPacketAsync(packet);
            }
            catch (Exception) when (_disposed)
            {
            }
        }

        #endregion

        #region helper methods
        
        private static async Task<T> GetAndVerifySignedDataAsync<T>(string signedDataJson) where T : class
        {
            var signedData = await JsonUtils.DeserializeAsync<SignedData>(signedDataJson);

            var dataJson = Config.DefaultEncoding.GetString(signedData.Data);
            var baseModel = await JsonUtils.DeserializeAsync<BaseModel>(dataJson);
            if (baseModel?.Sender?.PublicKey == null)
                return null;

            var publicKey = PemUtils.GetKeyFromPem(baseModel.Sender.PublicKey);
            var dataToVerify = Config.DefaultEncoding.GetBytes(Convert.ToBase64String(signedData.Data));
            var isValid = SignUtils.Verify(dataToVerify, signedData.Signature, publicKey);
            if (!isValid)
                return null;

            var text = Config.DefaultEncoding.GetString(signedData.Data);
            var model = await JsonUtils.DeserializeAsync<T>(text);
            return model;
        }

        private async Task<byte[]> CreateSignAndEncodeDataAsync(BaseModel model)
        {
            model.Sender = _publicIdentity;
            var signedData = new SignedData
            {
                Data = Config.DefaultEncoding.GetBytes(await JsonUtils.SerializeAsync(model))
            };
            
            var signature = SignUtils.Sign(Config.DefaultEncoding.GetBytes(Convert.ToBase64String(signedData.Data)), _personalIdentity.KeyPair.Private);
            signedData.Signature = signature;
            
            var json = await JsonUtils.SerializeAsync(signedData);
            var bytes = Config.DefaultEncoding.GetBytes(json);
            return bytes;
        }

        #endregion

        #region event incovators

        private void OnKeepAliveReceived(UserIdentity sender)
        {
            KeepAliveReceived?.Invoke(this, new KeepAliveReceivedEventArgs(sender));
        }

        protected void OnInviteReceived(string name, byte[] psk, UserIdentity sender)
        {
            InviteReceived?.Invoke(this, new InviteReceivedEventArgs(name, psk, sender));
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