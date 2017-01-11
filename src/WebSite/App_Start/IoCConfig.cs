using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Common.Logging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;
using WbMyFather.BLL;
using WbMyFather.BLL.Services;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DAL;
using WbMyFather.DAL.Context;
using WebSite.Mapping;

namespace WebSite
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
                    cfg.AddProfile<BllMapperConfig>();
                    cfg.AddProfile<MapperConfig>();
                });

                mapperConfiguration.AssertConfigurationIsValid();
                return mapperConfiguration.CreateMapper();

            }).As<IMapper>().SingleInstance();

            builder.RegisterType<DataContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IRepository<>));
            builder.RegisterType<UnitOfWorkBase>().As<IUnitOfWork>();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>();
            builder.RegisterType<WordsService>().As<IWordsService>();
            builder.RegisterType<BooksService>().As<IBooksService>();
        }
    }
}
