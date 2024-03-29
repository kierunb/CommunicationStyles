﻿namespace MassTransit.WebAPI.Models;

public class SubmitInvoiceCommand
{
    public Guid InvoiceId { get; init; }
    public required string InvoiceNumber { get; init; }
    public required decimal Amount { get; init; }
}
