using System;

namespace Flexx.Core.Protocol
{
    internal class KeepAliveMessagePacket : Packet
    {
        public KeepAliveMessagePacket(byte[] content)
        {
            Type = ModelType.KeepAlive;
            Content = content;
            Id = Guid.NewGuid();
        }

        public KeepAliveMessagePacket()
        {
            Type = ModelType.KeepAlive;
        }
    }
}