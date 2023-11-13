using MassTransit.Clients;
using MassTransit.Contracts;
using MassTransit.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(
            ILogger<TestController> logger)
        {
            _logger = logger;
        }

        // IPublishEndpoint to implement REPR (Request-Endpoint-Response) Pattern

        // Competing Consumers
        [HttpPost("ping")]
        public async Task<IActionResult> Ping([FromServices] IPublishEndpoint publishEndpoint)
        {
            await publishEndpoint.Publish(
                new PingMessage { PingId = NewId.NextGuid(), Message = "Ping message" });
            
            return Accepted();
        }

        [HttpPost("ping-error")]
        public async Task<IActionResult> PingError([FromServices] IPublishEndpoint publishEndpoint)
        {
            await publishEndpoint.Publish(
                new PingErrorMessage { PingId = NewId.NextGuid(), Message = "Ping error message" });

            return Accepted();
        }

        // Request/Reply
        [HttpPost("ping-pong")]
        public async Task<IActionResult> PingPong([FromServices] IRequestClient<PingRequest> requestClient)
        {
            var resonse = await requestClient.GetResponse<PingResponse>(
                               new PingRequest { Message = $"Ping request {DateTime.Now}" });
            
            return Ok(resonse.Message);
        }

        [HttpPost("process-raport")]
        public async Task<IActionResult> ProcessRaport([FromServices] IPublishEndpoint publishEndpoint)
        {
            await publishEndpoint.Publish(
                new ProcessRaportCommand { RaportId = NewId.NextGuid(), RaportName = "Raport 1", RaportContent = "Raport 1 content" });

            return Accepted();
        }

    }
}
