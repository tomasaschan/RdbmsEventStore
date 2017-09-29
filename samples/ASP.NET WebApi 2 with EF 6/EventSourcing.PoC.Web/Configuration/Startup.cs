using System.Web.Http;
using EventSourcing.PoC.Web.Configuration;
using Owin;
using Autofac;
using Autofac.Integration.WebApi;
using EventSourcing.PoC.Web.Configuration.ServiceRegistrations;

[assembly: Microsoft.Owin.OwinStartup(typeof(Startup))]

namespace EventSourcing.PoC.Web.Configuration
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ApplicationModule>();
            var container = builder.Build();

            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container)
            };
            config.MapHttpAttributeRoutes();

            app
                .UseAutofacMiddleware(container)
                .UseAutofacWebApi(config)
                .UseWebApi(config);
        }
    }
}