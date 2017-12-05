using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.PoC.Application.Events;
using MediatR;
using RdbmsEventStore;

namespace EventSourcing.PoC.Application.Messaging
{
    public class PostMessageCommandHandler : IRequestHandler<PostMessageCommand>
    {
        private readonly IEventStore<string, Event, IEventMetadata<string>> _eventWriter;

        public PostMessageCommandHandler(IEventStore<string, Event, IEventMetadata<string>> eventWriter)
        {
            _eventWriter = eventWriter;
        }

        public async Task Handle(PostMessageCommand command, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(command.Message))
            {
                throw new ArgumentException("Posting empty messages is not allowed");
            }

            var currentVersion = (await _eventWriter.Events("1")).Select(e => e.Timestamp)
                .DefaultIfEmpty(DateTimeOffset.MinValue)
                .Max() as DateTimeOffset?;

            if (currentVersion == DateTimeOffset.MinValue)
            {
                currentVersion = null;
            }
            await _eventWriter.Append("1", currentVersion, new MessagePostedEvent {Message = command.Message});
        }
    }
}