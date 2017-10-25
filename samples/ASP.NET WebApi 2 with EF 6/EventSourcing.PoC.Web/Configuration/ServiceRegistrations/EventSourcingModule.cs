using System.Collections.Generic;
using System.Reflection;
using Autofac;
using EventSourcing.PoC.Application.Messaging;
using EventSourcing.PoC.Persistence;
using RdbmsEventStore;
using RdbmsEventStore.EventRegistry;
using RdbmsEventStore.EntityFramework;
using RdbmsEventStore.Serialization;
using ApplicationEvent = EventSourcing.PoC.Application.Events.Event;
using PersistenceEvent = EventSourcing.PoC.Persistence.Event;
using Module = Autofac.Module;

namespace EventSourcing.PoC.Web.Configuration.ServiceRegistrations
{
    public class EventSourcingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IEventStream<,,>).GetTypeInfo().Assembly).AsImplementedInterfaces().SingleInstance();

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
                .RegisterType<EntityFrameworkEventStore<long, string, EventsContext, ApplicationEvent, IEventMetadata<string>, PersistenceEvent>>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventsContext>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DefaultEventSerializer<string, ApplicationEvent, PersistenceEvent>>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DefaultEventFactory<string, ApplicationEvent>>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<WriteLock<string>>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            base.Load(builder);
        }
    }
}