using System;
using System.Threading.Tasks;
using System.Web.Http;
using EventSourcing.PoC.Application.Messaging;
using MediatR;

namespace EventSourcing.PoC.Web.Controllers
{
    [RoutePrefix("messages")]
    public class MessageController : ApiController
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("latest")]
        public async Task<IHttpActionResult> GetCurrentMessage()
        {
            var message = await _mediator.Send(new LatestMessageQuery());
            return Json(message);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> PostNewMessage([FromBody] PostMessageCommand command)
        {
            try
            {
                await _mediator.Send(command);

                var latestMessage = await _mediator.Send(new LatestMessageQuery());
                return Json($"The last message is now {latestMessage}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
