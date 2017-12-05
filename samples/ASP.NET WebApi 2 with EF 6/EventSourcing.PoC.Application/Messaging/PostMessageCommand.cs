using MediatR;

namespace EventSourcing.PoC.Application.Messaging
{
    public class PostMessageCommand : IRequest
    {
        public PostMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}