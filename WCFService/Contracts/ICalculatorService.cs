using CoreWCF;

namespace WCF.Service.Contracts;

[ServiceContract]
public interface ICalculatorService
{
    [OperationContract]
    double Add(double n1, double n2);
    [OperationContract]
    double Subtract(double n1, double n2);
    [OperationContract]
    double Multiply(double n1, double n2);
    [OperationContract]
    double Divide(double n1, double n2);

    [OperationContract]
    int AnalyzePayload(PayloadRequest payload);
}
