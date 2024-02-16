using AutoMapper;
using MassTransit.Contracts;
using MassTransit.Contracts.InvoiceStateMachine;
using MassTransit.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly IMapper _mapper;

    public InvoiceController(
        ILogger<InvoiceController> logger,
        IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost("submit")]
    [ProducesResponseType(typeof(SubmitInvoiceCommand), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitInvoiceCommand submitInvoiceModel,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await publishEndpoint.Publish<SubmitInvoice>(_mapper.Map<SubmitInvoice>(submitInvoiceModel), cancellationToken);

        return Accepted(new SubmitInvoiceCommand
        {
            InvoiceId = submitInvoiceModel.InvoiceId,
            InvoiceNumber = submitInvoiceModel.InvoiceNumber,
            Amount = submitInvoiceModel.Amount
        });
    }

    [HttpGet("{invoiceId}")]
    [ProducesResponseType(typeof(InvoiceModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid invoiceId, CancellationToken cancellationToken, [FromServices] IRequestClient<GetInvoice> getInvoiceClient)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await getInvoiceClient.GetResponse<CurrentInvoiceState>(new
        {
            invoiceId,
        }, cancellationToken);

        return response switch
        {
            (_, CurrentInvoiceState x) => Ok(new InvoiceModel
            {
                InvoiceId = x.InvoiceId,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceDate = x.InvoiceDate,
                Amount = x.Amount,
                Status = x.Status
            }),
            _ => NotFound()
        };
    }

    [HttpPost("accept")]
    [ProducesResponseType(typeof(AcceptInvoiceCommand), StatusCodes.Status200OK)]
    public async Task<IActionResult> Accept(
        [FromBody] AcceptInvoiceCommand acceptInvoiceModel,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await publishEndpoint.Publish<AcceptInvoice>(_mapper.Map<AcceptInvoice>(acceptInvoiceModel), cancellationToken);

        return Accepted(new SubmitInvoiceCommand
        {
            InvoiceId = acceptInvoiceModel.InvoiceId,
            InvoiceNumber = acceptInvoiceModel.InvoiceNumber,
            Amount = acceptInvoiceModel.Amount
        });
    }


    [HttpPost("finalize")]
    [ProducesResponseType(typeof(FinalizeInvoiceCommand), StatusCodes.Status200OK)]
    public async Task<IActionResult> Finalize(
    [FromBody] FinalizeInvoiceCommand finalizeInvoiceModel,
    [FromServices] IPublishEndpoint publishEndpoint,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await publishEndpoint.Publish<FinalizeInvoice>(_mapper.Map<FinalizeInvoice>(finalizeInvoiceModel), cancellationToken);

        return Accepted(new FinalizeInvoiceCommand
        {
            InvoiceId = finalizeInvoiceModel.InvoiceId,
            InvoiceNumber = finalizeInvoiceModel.InvoiceNumber
        });
    }
}
