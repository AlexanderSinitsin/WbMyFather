using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Mvc.Helpers
{
    public static class TableHelper
    {
        public static IHtmlString Json(this HtmlHelper htmlHelper, object obj)
        {
            return new HtmlString(JsonConvert.SerializeObject(obj));
        }
    }
}
