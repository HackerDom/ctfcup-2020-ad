using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QueenOfHearts.CoreLibrary.Serialization
{
    public static class JsonSerializationHelper
    {
        public static TValue FromJson<TValue>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<TValue>(jsonString);
        }

        public static async Task<TValue> FromJsonAsync<TValue>(this Stream stream)
        {
            var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            return text.FromJson<TValue>();
        }

        public static string ToJson<TValue>(this TValue item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}