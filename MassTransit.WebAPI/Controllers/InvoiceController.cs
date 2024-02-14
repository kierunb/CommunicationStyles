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
    [ProducesResponseType(typeof(SubmitInvoiceModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitInvoiceModel submitInvoiceModel,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await publishEndpoint.Publish<SubmitInvoice>(_mapper.Map<SubmitInvoice>(submitInvoiceModel), cancellationToken);

        return Accepted(new SubmitInvoiceModel
        {
            InvoiceId = submitInvoiceModel.InvoiceId,
            InvoiceNumber = submitInvoiceModel.InvoiceNumber,
            Amount = submitInvoiceModel.Amount
        });
    }

    [HttpPost("accept")]
    [ProducesResponseType(typeof(AcceptInvoiceModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Accept(
        [FromBody] AcceptInvoiceModel acceptInvoiceModel,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await publishEndpoint.Publish<AcceptInvoice>(_mapper.Map<AcceptInvoice>(acceptInvoiceModel), cancellationToken);

        return Accepted(new SubmitInvoiceModel
        {
            InvoiceId = acceptInvoiceModel.InvoiceId,
            InvoiceNumber = acceptInvoiceModel.InvoiceNumber,
            Amount = acceptInvoiceModel.Amount
        });
    }
}
