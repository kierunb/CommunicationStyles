using System.ComponentModel.DataAnnotations;

namespace MassTransit.WebAPI.Models;

public record SubmitOrderModel
{
    [Required]
    public Guid OrderId { get; init; }

    [Required]
    [MinLength(6)]
    public string OrderNumber { get; init; }
}
