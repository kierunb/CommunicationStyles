using CalculatorServiceReference;

Console.WriteLine("Hello, WCF Client!");

var proxy = new CalculatorServiceClient();

Console.WriteLine($"Calling Add(2, 3). Result: {await proxy.AddAsync(2,3)}");

Console.WriteLine($"Calling AnalyzePayload(). Result:  {await proxy.AnalyzePayloadAsync(new PayloadRequest { Name = "Payload", Message = "data" })}");

Console.ReadLine();