using AutoMapper;
using MassTransit.Contracts.InvoiceStateMachine;
using MassTransit.WebAPI.Models;

namespace MassTransit.WebAPI.Profiles;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<SubmitInvoiceModel, SubmitInvoice>()
            .ReverseMap();

        CreateMap<AcceptInvoiceModel, AcceptInvoice>()
            .ReverseMap();
    }
}
