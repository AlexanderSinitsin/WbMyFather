using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WbMyFather.WebSite.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected BaseController(Common.Logging.ILog someService)
        {
            SomeService = someService;
        }

        protected Common.Logging.ILog SomeService { get; }

        /// <summary>
        /// Сессия
        /// </summary>
        protected SessionManager SessionManager => sessionManager ?? (sessionManager = new SessionManager(Session));
        private SessionManager sessionManager;

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is SecurityException)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new PartialViewResult
                    {
                        ViewName = "_AccessDeniedPartial",
                        ViewData = new ViewDataDictionary()
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "AccessDenied",
                        ViewData = new ViewDataDictionary()
                    };
                }
                Response.StatusCode = 403;
                filterContext.ExceptionHandled = true;

                return;
            }

            base.OnException(filterContext);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            var json = base.Json(data, contentType, contentEncoding, behavior);
            json.MaxJsonLength = int.MaxValue;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
