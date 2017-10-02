using Autofac;
using Autofac.Integration.WebApi;
using EventSourcing.PoC.Application.Messaging;

namespace EventSourcing.PoC.Web.Configuration.ServiceRegistrations
{
    public class ApplicationModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(typeof(Startup).Assembly);

            builder.RegisterModule<EventSourcingModule>();
            builder.RegisterModule(new MediatRModule(typeof(PostMessageCommand)));
        }
    }
}