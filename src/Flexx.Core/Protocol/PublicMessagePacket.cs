using System;
using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class PublicMessagePacket : Packet
    {
        [JsonProperty(PropertyName = "chatRoom")]
        public byte[] ChatRoom { get; set; }

        public PublicMessagePacket(byte[] content, byte[] chatRoom)
        {
            Type = ModelType.PublicMessage;
            Content = content;
            ChatRoom = chatRoom;
            Id = Guid.NewGuid();
        }

        public PublicMessagePacket()
        {
            Type = ModelType.PublicMessage;
        }
    }
}