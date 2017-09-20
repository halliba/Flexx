using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    internal class KeepAlive : BaseModel
    {
        [JsonProperty(PropertyName = "timeStamp")]
        public long TimeStamp { get; set; }
    }
}