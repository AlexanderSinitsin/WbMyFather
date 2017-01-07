using System;
using System.Linq;
using System.Web.Mvc;

namespace Smik.WebSite.Mvc.Helpers
{
    public static class MenuHelper
    {
        public static string IsSelected(this HtmlHelper html, string actions = null, string cssClass = "active")
        {
            if (string.IsNullOrWhiteSpace(actions))
            {
                return cssClass;
            }

            var viewContext = html.ViewContext.IsChildAction ? html.ViewContext.ParentActionViewContext : html.ViewContext;
            var currentAction = ((string)viewContext.RouteData.Values["action"]).ToLowerInvariant();
            var currentController = ((string)viewContext.RouteData.Values["controller"]).ToLowerInvariant();

            return actions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a =>
            {
                string controller = null;
                string action = null;

                var pair = a.Split('.').Select(s => s.Trim().ToLowerInvariant()).ToList();
                if (pair.Count > 0)
                {
                    controller = pair[0];
                }

                if (pair.Count > 1)
                {
                    action = pair[1];
                }

                return new { Controller = controller, Action = action };
            })
            .Any(action => action.Controller == currentController && (action.Action == null || action.Action == currentAction))
                ? cssClass
                : string.Empty;
        }
    }
}
