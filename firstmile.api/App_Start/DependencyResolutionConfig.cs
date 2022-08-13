using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using firstmile.data;
using firstmile.domain.Services;
using firstmile.services.Interface;
using firstmile.services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace firstmile.api.App_Start
{
    public class DependencyResolutionConfig
    {
        public static void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            DependencyResolutionConfig.RegisterServices(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container);
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<LookUpService>().As<ILookUpService>().InstancePerRequest();
            builder.RegisterType<EquipmentService>().As<IEquipmentService>().InstancePerRequest();
            builder.RegisterType<BookingService>().As<IBookingService>().InstancePerRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerRequest();
            builder.RegisterType<LoggerService>().As<ILoggerService>().InstancePerRequest();
            builder.RegisterType<UsageService>().As<IUsageService>().InstancePerRequest();
        }
    }
}