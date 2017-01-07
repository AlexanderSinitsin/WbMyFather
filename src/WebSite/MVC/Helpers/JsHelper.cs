using Newtonsoft.Json;

namespace WebSite.Mvc.Helpers
{
    public static class JsHelper
    {
        public static string Serialize(object o)
        {
            var serializeString = JsonConvert.SerializeObject(o);
            return serializeString;
        }
    }
}