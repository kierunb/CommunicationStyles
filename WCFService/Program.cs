using CoreWCF;
using CoreWCF.Configuration;
using WCF.Service.Contracts;
using WCF.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// enable CoreWCF Services, with metadata (WSDL) support
builder.Services
    .AddServiceModelServices()
    .AddServiceModelMetadata();


var app = builder.Build();

app.UseServiceModel(builder =>
{
    // Add the Calculator Service
    builder.AddService<CalculatorService>(serviceOptions => { })
    // Add BasicHttpBinding endpoint
        .AddServiceEndpoint<CalculatorService, ICalculatorService>(new BasicHttpBinding(), "/CalculatorService/basicHttp");


    // Configure WSDL to be available
    var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});


app.MapGet("/", () => "Hello from WCF Service!");

app.Run();
