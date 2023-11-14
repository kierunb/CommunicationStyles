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
        // https://masstransit.io/documentation/concepts/producers
        // Prefer Publish over Send to avoid coupling between the sender and the receiver and fetching queue name/address
        // https://stackoverflow.com/questions/62713786/masstransit-endpointconvention-azure-service-bus/62714778#62714778

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromServices] IBus bus, CancellationToken ct)
        {
            // need to know the queue name/address
            // long address:
            // rabbitmq://localhost/input-queue
            // rabbitmq://localhost/input-queue?durable=false (temporary=true etc.)
            // short address:
            // queue:input-queue

            var endpoint = await bus.GetSendEndpoint(new Uri("queue:ping-queue"));
            await endpoint.Send(new PingMessage { PingId = NewId.NextGuid(), Message = "Ping Messege" }, ct);

            return Accepted();
        }

        [HttpPost("send-timeout")]
        public async Task<IActionResult> SendTimeout([FromServices] IBus bus, CancellationToken ct)
        {
            var endpoint = await bus.GetSendEndpoint(new Uri("queue:ping-queue"));

            var timeout = TimeSpan.FromSeconds(10);
            using var source = new CancellationTokenSource(timeout);

            await endpoint.Send(new PingMessage { PingId = NewId.NextGuid(), Message = "Ping Messege" }, source.Token);

            return Accepted();
        }

        // Competing Consumers, Publish/Subscribe patterns
        [HttpPost("ping")]
        public async Task<IActionResult> Ping([FromServices] IPublishEndpoint publishEndpoint, CancellationToken ct)
        {
            await publishEndpoint.Publish(
                new PingMessage { PingId = NewId.NextGuid(), Message = "Ping message" }, ct);
            
            return Accepted();
        }

        [HttpPost("ping-error")]
        public async Task<IActionResult> PingError([FromServices] IPublishEndpoint publishEndpoint, CancellationToken ct)
        {
            await publishEndpoint.Publish(
                new PingErrorMessage { PingId = NewId.NextGuid(), Message = "Ping error message" }, ct);

            return Accepted();
        }

        // Request/Reply
        [HttpPost("ping-pong")]
        public async Task<IActionResult> PingPong([FromServices] IRequestClient<PingRequest> requestClient, CancellationToken ct)
        {
            var resonse = await requestClient.GetResponse<PingResponse>(
                               new PingRequest { Message = $"Ping request {DateTime.Now}" }, ct);
            
            return Ok(resonse.Message);
        }

        [HttpPost("process-raport")]
        public async Task<IActionResult> ProcessRaport([FromServices] IPublishEndpoint publishEndpoint, CancellationToken ct)
        {
            await publishEndpoint.Publish(
                new ProcessRaportCommand { RaportId = NewId.NextGuid(), RaportName = "Raport 1", RaportContent = "Raport 1 content" }, ct);

            return Accepted();
        }

    }
}
