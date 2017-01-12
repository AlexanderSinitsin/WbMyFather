using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace WebSite.Mvc.Helpers
{
    public static class LabelForHelper
    {
        public static IHtmlString EditLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            attributes["class"] = "control-label" + attributes["class"];
            return htmlHelper.LabelFor(expression, attributes);
        }
    }
}
