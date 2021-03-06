﻿using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Flexx.Core.Utils;

namespace Flexx.Core
{
    public class PublicChatRoom : ChatRoom
    {
        private readonly CryptoChatAdapter _cryptoAdapter;

        public byte[] PreSharedKey { get; }

        public string Name { get; }

        private readonly byte[] _nameBytes;

        internal PublicChatRoom(CryptoChatAdapter cryptoAdapter, string name, string password)
        {
            Name = name;
            _nameBytes = Config.DefaultEncoding.GetBytes(Name);
            _cryptoAdapter = cryptoAdapter;
            PreSharedKey = GeneratePreSharedKey(password);
        }

        internal PublicChatRoom(CryptoChatAdapter cryptoAdapter, string name, byte[] preSharedKey)
        {
            Name = name;
            _nameBytes = Config.DefaultEncoding.GetBytes(Name);
            _cryptoAdapter = cryptoAdapter;
            PreSharedKey = preSharedKey;
        }

        internal void OnPublicMessageReceived(MessageReceivedEventArgs args) => OnMessageReceived(args);
        
        public bool Equals(byte[] chatRoomIdentifier)
        {
            var decrypted = CryptUtils.AesDecryptBytes(chatRoomIdentifier, PreSharedKey);
            if (decrypted == null) return false;
            var encoded = Config.DefaultEncoding.GetString(decrypted);
            return encoded == Name;
        }

        public bool Equals(string name, string password)
        {
            return Name == name && GeneratePreSharedKey(password).SequenceEqual(PreSharedKey);
        }

        public bool Equals(string name, byte[] preSharedKey)
        {
            return Name == name && preSharedKey.SequenceEqual(PreSharedKey);
        }

        internal byte[] GetEncryptedIdentifier()
        {
            var encrypted = CryptUtils.AesEncryptByteArray(_nameBytes, PreSharedKey);
            return encrypted;
        }

        internal static byte[] GeneratePreSharedKey(string password)
        {
            return new SHA256Managed().ComputeHash(Config.DefaultEncoding.GetBytes(password));
        }

        public async Task SendMessageAsync(string content)
        {
            await _cryptoAdapter.SendPublicMessageAsync(this, content);
        }

        public async Task SendInviteAsync(UserIdentity user)
        {
            await _cryptoAdapter.SendInviteAsync(Name, PreSharedKey, user);
        }

        public void Leave()
        {
            _cryptoAdapter.LeavePublicChatRoom(this);
        }
    }
}