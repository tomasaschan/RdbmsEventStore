using System;
using System.Threading.Tasks;
using EventSourcing.PoC.Application.Events;
using MediatR;
using RdbmsEventStore;

namespace EventSourcing.PoC.Application.Messaging
{
    public class PostMessageCommandHandler : IAsyncRequestHandler<PostMessageCommand>
    {
        private readonly IEventWriter<long, Event> _eventWriter;

        public PostMessageCommandHandler(IEventWriter<long, Event> eventWriter)
        {
            _eventWriter = eventWriter;
        }

        public Task Handle(PostMessageCommand command)
            => !string.IsNullOrWhiteSpace(command.Message)
                ? _eventWriter.Commit(1, command.CurrentVersion, new MessagePostedEvent { Message = command.Message })
                : throw new ArgumentException("Posting empty messages is not allowed");
    }
}