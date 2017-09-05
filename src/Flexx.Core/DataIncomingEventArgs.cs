using System;
using Flexx.Core.Protocol;

namespace Flexx.Core
{
    internal class PacketIncomingEventArgs : EventArgs
    {
        public string PacketJson { get; }

        public ModelType PacketType { get; }

        public PacketIncomingEventArgs(string packetJson, ModelType packetType)
        {
            PacketJson = packetJson;
            PacketType = packetType;
        }
    }
}