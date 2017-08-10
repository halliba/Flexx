using System;
using System.Net;

namespace Flexx.Core
{
    internal class DataIncomingEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public IPEndPoint RemoteEndPoint { get; }

        public DataIncomingEventArgs(byte[] data, IPEndPoint remoteEndPoint)
        {
            Data = data;
            RemoteEndPoint = remoteEndPoint;
        }
    }
}