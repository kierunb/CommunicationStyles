﻿namespace MassTransit.Contracts;

public record Order
{
    public Guid OrderId { get; init; }
    public required string OrderNumber { get; init; }
    public required string Status { get; init; }
}
