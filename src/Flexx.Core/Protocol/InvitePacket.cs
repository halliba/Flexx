using System;
using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class InvitePacket : Packet
    {
        [JsonProperty(PropertyName = "aesKey")]
        public byte[] AesKey { get; set; }

        public InvitePacket(byte[] content, byte[] aesKey)
        {
            Type = ModelType.Invite;
            Content = content;
            AesKey = aesKey;
            Id = Guid.NewGuid();
        }

        public InvitePacket()
        {
            Type = ModelType.Invite;
        }
    }
}