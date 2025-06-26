using Newtonsoft.Json.Linq;
using System.IO;

namespace ApiTestFramework.Support
{
    public static class ReqresConfig
    {
        public static string GetApiKey()
        {
            var json = File.ReadAllText("appsettings.json");
            var config = JObject.Parse(json);
            return config["ReqresApiKey"]?.ToString();
        }
    }
}