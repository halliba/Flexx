using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flexx.Core
{
    public class PublicChatRoom : ChatRoom
    {
        private readonly CryptoChatAdapter _cryptoAdapter;

        internal readonly byte[] PreSharedKey;
        internal readonly byte[] Identifier;

        public string Name { get; }

        internal PublicChatRoom(CryptoChatAdapter cryptoAdapter, string name, string preSharedKey)
        {
            Name = name;
            _cryptoAdapter = cryptoAdapter;
            PreSharedKey = GeneratePreSharedKey(preSharedKey);
            Identifier = GetChatRoomIdentifier(name, PreSharedKey);
        }

        internal void OnPublicMessageReceived(MessageReceivedEventArgs args) => OnMessageReceived(args);

        private static byte[] GetChatRoomIdentifier(string name, byte[] preSharedKey)
        {
            var nameHash = new SHA256Managed().ComputeHash(Encoding.Unicode.GetBytes(name));
            var encrypted = CryptUtils.AesEncryptByteArray(nameHash, preSharedKey);
            return encrypted;
        }

        private static byte[] GeneratePreSharedKey(string preSharedKey)
        {
            return new SHA256Managed().ComputeHash(Encoding.Unicode.GetBytes(preSharedKey));
        }

        public async Task SendMessageAsync(string content)
        {
            await _cryptoAdapter.SendPublicMessageAsync(this, content);
        }
    }
}