using System;
using System.Threading.Tasks;
using EventSourcing.PoC.Application.Events;
using MediatR;
using RdbmsEventStore;

namespace EventSourcing.PoC.Application.Messaging
{
    public class PostMessageCommandHandler : IAsyncRequestHandler<PostMessageCommand>
    {
        private readonly IEventStream<string, Event, IEventMetadata<string>> _eventWriter;

        public PostMessageCommandHandler(IEventStream<string, Event, IEventMetadata<string>> eventWriter)
        {
            _eventWriter = eventWriter;
        }

        public Task Handle(PostMessageCommand command)
            => !string.IsNullOrWhiteSpace(command.Message)
                ? _eventWriter.Append("1", command.CurrentVersion, new MessagePostedEvent { Message = command.Message })
                : throw new ArgumentException("Posting empty messages is not allowed");
    }
}