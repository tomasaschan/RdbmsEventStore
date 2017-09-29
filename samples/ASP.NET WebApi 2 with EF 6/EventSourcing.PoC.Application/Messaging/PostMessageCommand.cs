using MediatR;

namespace EventSourcing.PoC.Application.Messaging
{
    public class PostMessageCommand : IRequest
    {
        public PostMessageCommand(string message, long currentVersion)
        {
            Message = message;
            CurrentVersion = currentVersion;
        }

        public string Message { get; }
        public long CurrentVersion { get; }
    }
}