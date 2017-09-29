using Autofac;
using EventSourcing.PoC.Application.Messaging;

namespace EventSourcing.PoC.Web.Configuration.ServiceRegistrations
{
    public class ApplicationModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<EventSourcingModule>();
            builder.RegisterModule(new MediatRModule(typeof(PostMessageCommand)));
        }
    }
}