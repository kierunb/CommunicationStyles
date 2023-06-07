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

        [HttpPost("ping")]
        public async Task<IActionResult> Ping([FromServices] IPublishEndpoint publishEndpoint)
        {
            await publishEndpoint.Publish(
                new PingMessage { PingId = NewId.NextGuid(), Message = "Ping message" });
            
            return Accepted();
        }

    }
}
