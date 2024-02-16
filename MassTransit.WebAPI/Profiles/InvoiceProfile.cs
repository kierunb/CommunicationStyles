using AutoMapper;
using MassTransit.Contracts.DocumentApprovalEvents;
using MassTransit.Contracts.InvoiceStateMachine;
using MassTransit.WebAPI.Models;
using MassTransit.WebAPI.Requests;

namespace MassTransit.WebAPI.Profiles;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<SubmitInvoiceCommand, SubmitInvoice>()
            .ReverseMap();

        CreateMap<AcceptInvoiceCommand, AcceptInvoice>()
            .ReverseMap();

        CreateMap<FinalizeInvoiceCommand, FinalizeInvoice>()
            .ReverseMap();

        CreateMap<CurrentInvoiceState, InvoiceModel>()
            .ReverseMap();

        CreateMap<DocumentSubmissionRequest, DocumentSubmitted>()
            .ReverseMap();
    }
}
