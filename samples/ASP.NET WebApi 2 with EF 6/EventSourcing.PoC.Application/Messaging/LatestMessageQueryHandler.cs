using System.Threading.Tasks;
using EventSourcing.PoC.Application.Events;
using MediatR;
using RdbmsEventStore;

namespace EventSourcing.PoC.Application.Messaging
{
    public class LatestMessageQueryHandler : IAsyncRequestHandler<LatestMessageQuery, string>
    {
        private readonly IEventStream<long, Event> _eventStream;
        private readonly IMaterializer _materializer;

        public LatestMessageQueryHandler(IEventStream<long, Event> eventStream, IMaterializer materializer)
        {
            _eventStream = eventStream;
            _materializer = materializer;
        }

        public async Task<string> Handle(LatestMessageQuery _)
        {
            var events = await _eventStream.Events(1);
            var message = _materializer.Unfold("", events, (current, evt) =>
            {
                switch (evt)
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