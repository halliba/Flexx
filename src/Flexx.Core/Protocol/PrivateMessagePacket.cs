using System;
using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class PrivateMessagePacket : Packet
    {
        [JsonProperty(PropertyName = "aesKey")]
        public byte[] AesKey { get; set; }

        public PrivateMessagePacket(byte[] content, byte[] aesKey)
        {
            Type = ModelType.PrivateMessage;
            Content = content;
            AesKey = aesKey;
            Id = Guid.NewGuid();
        }

        public PrivateMessagePacket()
        {
            Type = ModelType.PrivateMessage;
        }
    }
}