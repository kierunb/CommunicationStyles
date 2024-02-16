namespace MassTransit.WebAPI.Requests;

public class DocumentSubmissionRequest
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = default!;
    public string DocumentType { get; init; } = default!;
    public string DocumentContent { get; init; } = default!;
}
