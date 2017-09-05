using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Flexx.Core.Protocol
{
    internal class Packet
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public ModelType Type { get; set; }

        [JsonProperty(PropertyName = "content")]
        public byte[] Content { get; set; }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
    }
}