using System.Collections.Generic;
using System.Reflection;
using Autofac;
using EventSourcing.PoC.Application.Events;
using EventSourcing.PoC.Application.Messaging;
using EventSourcing.PoC.Persistence;
using RdbmsEventStore;
using RdbmsEventStore.EventRegistry;
using RdbmsEventStore.EntityFramework;
using Module = Autofac.Module;

namespace EventSourcing.PoC.Web.Configuration.ServiceRegistrations
{
    public class EventSourcingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IEventStore<,>).GetTypeInfo().Assembly).AsImplementedInterfaces().SingleInstance();

            builder.RegisterInstance(new TranslatingEventRegistry(
                new Dictionary<string, string>
                {
                    // If events are renamed (e.g. if switching from SimpleEventRegistry to AssemblyEventRegistry)
                    // add translations here. Example:
                    { "OldEventName", "NewEventName" }
                },
                new AssemblyEventRegistry(typeof(MessagePostedEvent), type => type.Name.EndsWith("Event"))))
                .AsImplementedInterfaces().SingleInstance();

            builder
                .RegisterType<EntityFrameworkEventStore<long, EventsContext, Event>>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventsContext>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventFactory<long, Event>>()
                .AsImplementedInterfaces()
                .SingleInstance();

            base.Load(builder);
        }
    }
}