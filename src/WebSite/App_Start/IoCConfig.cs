using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebSite.Mapping;

namespace WebSite.App_Start
{
    public static class IoCConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterDependencies();
            builder.RegisterControllers(typeof(IoCConfig).Assembly);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
        private static void RegisterDependencies(this ContainerBuilder builder)
        {
            builder.Register(c => LogManager.GetLogger("Default")).As<ILog>().SingleInstance();

            //AutoMapper
            builder.Register(c =>
            {
                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MapperConfig>();
                });

                mapperConfiguration.AssertConfigurationIsValid();
                return mapperConfiguration.CreateMapper();

            }).As<IMapper>().SingleInstance();
        }
    }
}
