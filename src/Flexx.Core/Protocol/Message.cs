using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    public class Message : BaseModel
    {
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public long TimeStamp { get; set; }
    }
}