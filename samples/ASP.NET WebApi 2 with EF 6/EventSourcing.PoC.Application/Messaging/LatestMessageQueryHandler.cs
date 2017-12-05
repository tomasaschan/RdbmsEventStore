using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.PoC.Application.Events;
using MediatR;
using RdbmsEventStore;

namespace EventSourcing.PoC.Application.Messaging
{
    public class LatestMessageQueryHandler : IRequestHandler<LatestMessageQuery, string>
    {
        private readonly IEventStore<string, Event, IEventMetadata<string>> _eventStream;

        public LatestMessageQueryHandler(IEventStore<string, Event, IEventMetadata<string>> eventStream)
        {
            _eventStream = eventStream;
        }

        public async Task<string> Handle(LatestMessageQuery _, CancellationToken token)
        {
            var events = await _eventStream.Events("1");
            var message = events.Aggregate("", (current, evt) =>
            {
                switch (evt.Payload)
                {
                    case MessagePostedEvent post:
                        return post.Message;
                    default:
                        return current;
                }
            });
            return message;
        }
    }
}