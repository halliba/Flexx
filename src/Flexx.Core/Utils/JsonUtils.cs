using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flexx.Core.Utils
{
    internal static class JsonUtils
    {
        public static async Task<T> DeserializeAsync<T>(string value)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                catch (Exception)
                {
                    return default(T);
                }
            });
        }

        public static async Task<string> SerializeAsync(object value)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(value));
        }
    }
}