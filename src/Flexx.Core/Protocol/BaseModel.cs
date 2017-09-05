using Newtonsoft.Json;

namespace Flexx.Core.Protocol
{
    public class BaseModel
    {
        [JsonProperty(PropertyName = "sender")]
        public UserIdentity Sender { get; set; }
    }
}