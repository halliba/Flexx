using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flexx.Core
{
    internal static class JsonUtils
    {
        public static async Task<T> DeserializeAsync<T>(string value)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(value));
        }

        public static async Task<string> SerializeAsync(object value)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(value));
        }
    }
}