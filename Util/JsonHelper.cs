using Newtonsoft.Json;

namespace BinanceWebSocket.Util
{
    public static class JsonHelper
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T? Deserialize<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}