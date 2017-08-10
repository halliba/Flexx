namespace Flexx.Core.Api
{
    internal class Transport
    {
        public ModelType Type { get; set; }

        public byte[] Data { get; set; }

        public byte[] Signature { get; set; }

        public string PublicKey { get; set; }
    }
}