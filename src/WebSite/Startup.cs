using Microsoft.Owin;
using Owin;
using System.Web.Mvc;
using System.Web.Routing;
using WbMyFather.BLL;
using WebSite;

[assembly: OwinStartup(typeof(WebSite.Startup))]
namespace WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Инициализация БД
            //BllServiceInitializer.Initialize();

            /*IoCConfig.RegisterDependencies();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);*/
        }
    }
}
