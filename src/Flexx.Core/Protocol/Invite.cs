using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class Invite : BaseModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "preSharedKey")]
        public byte[] PreSharedKey { get; set; }
    }
}