using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class SignedData
    {
        [JsonProperty(PropertyName = "data")]
        public byte[] Data { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public byte[] Signature { get; set; }
    }
}