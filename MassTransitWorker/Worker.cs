using MassTransit.Contracts;

namespace MassTransit.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _bus;

        public Worker(ILogger<Worker> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                //await _bus.Publish(new GettingStartedMessage { Number = DateTime.Now.Second,  Payload = $"The time is {DateTimeOffset.Now}" }, stoppingToken);             
                //_logger.LogInformation("GettingStartedMessage sent at: {time}", DateTimeOffset.Now);
                
                //_logger.LogInformation("Waiting for messages at: {time}", DateTimeOffset.Now);

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }
    }
}