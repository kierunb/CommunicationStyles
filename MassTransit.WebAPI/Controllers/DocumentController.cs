using AutoMapper;
using MassTransit.Contracts.DocumentApprovalEvents;
using MassTransit.WebAPI.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController(ILogger<DocumentController> logger, IMapper mapper) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] DocumentSubmissionRequest documentSubmissionRequest, 
            [FromServices] IPublishEndpoint publishEndpoint, 
            CancellationToken cancellationToken)
        {
            
            //var msg = mapper.Map<DocumentSubmitted>(documentSubmissionRequest);


            await publishEndpoint.Publish(mapper.Map<DocumentSubmitted>(documentSubmissionRequest), cancellationToken);

            return Accepted();
        }
    }
}
