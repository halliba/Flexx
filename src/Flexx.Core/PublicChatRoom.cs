using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flexx.Core
{
    public class PublicChatRoom : ChatRoom
    {
        private readonly CryptoChatAdapter _cryptoAdapter;

        internal readonly byte[] PreSharedKey;

        public string Name { get; }

        private readonly byte[] _nameBytes;

        internal PublicChatRoom(CryptoChatAdapter cryptoAdapter, string name, string preSharedKey)
        {
            Name = name;
            _nameBytes = Config.DefaultEncoding.GetBytes(Name);
            _cryptoAdapter = cryptoAdapter;
            PreSharedKey = GeneratePreSharedKey(preSharedKey);
        }

        internal void OnPublicMessageReceived(MessageReceivedEventArgs args) => OnMessageReceived(args);
        
        public bool Equals(byte[] chatRoomIdentifier)
        {
            var decrypted = CryptUtils.AesDecryptBytes(chatRoomIdentifier, PreSharedKey);
            if (decrypted == null) return false;
            var encoded = Config.DefaultEncoding.GetString(decrypted);
            return encoded == Name;
        }

        internal byte[] GetEncryptedIdentifier()
        {
            var encrypted = CryptUtils.AesEncryptByteArray(_nameBytes, PreSharedKey);
            return encrypted;
        }

        private static byte[] GeneratePreSharedKey(string preSharedKey)
        {
            return new SHA256Managed().ComputeHash(Config.DefaultEncoding.GetBytes(preSharedKey));
        }

        public async Task SendMessageAsync(string content)
        {
            await _cryptoAdapter.SendPublicMessageAsync(this, content);
        }
    }
}