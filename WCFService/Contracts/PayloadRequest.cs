using System.Runtime.Serialization;

namespace WCF.Service.Contracts;

[DataContract]
public class PayloadRequest
{
    [DataMember]
    public string? Name { get; set; }
    [DataMember]
    public string? Message { get; set; }
}
