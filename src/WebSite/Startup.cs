using Microsoft.Owin;
using Owin;
using System.Web.Mvc;
using System.Web.Routing;
using WebSite.App_Start;

[assembly: OwinStartup(typeof(WebSite.Startup))]
namespace WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IoCConfig.RegisterDependencies();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
