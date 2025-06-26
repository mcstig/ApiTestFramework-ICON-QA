using Newtonsoft.Json.Linq;
using System.IO;

namespace YourProjectNamespace.Support
{
    public static class WeatherstackConfig
    {
        public static string GetApiKey()
        {
            var json = File.ReadAllText("appsettings.json");
            var config = JObject.Parse(json);
            return config["WeatherstackApiKey"]?.ToString();
        }
    }
}
