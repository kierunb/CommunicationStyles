# Communication Styles & Technologies

To see a demo of WCF and/or GRPC, run XYZService first and then corresponding client project.
Demo of Mass Transit messaging requires RabbitMQ to be installed and running (e.g. in container).

Preconfigured Docker image maintained by the MassTransit team. 
The container image includes the delayed exchange plug-in and the management interface is enabled.
```bash
docker run --name rabbitmq -d -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

## SOAP & WSDL over HTTP - WCF Core
- [Service-oriented Architecture (SOA)](https://www.wikiwand.com/en/Service-oriented_architecture)
- [SOAP](https://www.wikiwand.com/en/SOAP)
- [WSDL](https://www.wikiwand.com/en/Web_Services_Description_Language)
- [WCF](https://learn.microsoft.com/en-us/dotnet/framework/wcf/whats-wcf)
- [WCF Core](https://github.com/CoreWCF/CoreWCF)


## GRPC over HTTP/2 - GRPC Core
- [GRPC](https://grpc.io/)
- [grpc-dotnet](https://learn.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-7.0)
- [Protocol Buffers](https://protobuf.dev/)
- [HTTP/2](https://www.oreilly.com/content/http2-a-new-excerpt/)


## REST over HTTP - ASP.NET Core Web API
- [REST](https://restfulapi.net/)
- [OData](https://www.odata.org/getting-started/understand-odata-in-6-steps/)
- [OData](https://learn.microsoft.com/en-us/odata/overview)
- [RESTful web API design](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Swagger & REST API Demo](https://petstore.swagger.io/)
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md#142-return-codes-429-vs-503)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis?view=aspnetcore-7.0)


## GraphQL over HTTP - ASP.NET Core Web API
- [GraphQL](https://graphql.org/)
- [GraphQL Intro](https://graphql.org/learn/)
- [Hot Chocolate](https://chillicream.com/docs/hotchocolate/v13)
- [Data API builder](https://learn.microsoft.com/en-us/azure/data-api-builder/overview-to-data-api-builder)


## SignalR over HTTP - ASP.NET Core Web API
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-7.0)
- [SignalR Demos](http://philssignalrdemo.azurewebsites.net/)
- [WebSockets](https://www.wikiwand.com/en/WebSocket)
- [Server-Sent Events](https://www.wikiwand.com/en/Server-sent_events)
- [Long Polling](https://stackoverflow.com/questions/11077857/what-are-long-polling-websockets-server-sent-events-sse-and-comet)

## Messaging with EasyNetQ over RabbitMQ
- [EasyNetQ](https://github.com/EasyNetQ/EasyNetQ)
- [Docs](https://github.com/EasyNetQ/EasyNetQ/wiki/Introduction)

### Demos
- **Competing Consumers**
    - Start 2+ Subscribers and 1 Publisher
- **Temporal Decoupling**
    - Start 1 Publisher, send some messages then start 1 Subsriber

## Messaging with MassTransit over RabbitMQ
- [Asynchronous message-based communication](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/asynchronous-message-based-communication)
- [Messaging Patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/category/messaging)
- [RabbitMQ](https://www.rabbitmq.com/#features)
- [RabbitMQ Introduction](https://www.cloudamqp.com/blog/part1-rabbitmq-for-beginners-what-is-rabbitmq.html)
- [AMQP Model](https://www.rabbitmq.com/tutorials/amqp-concepts.html)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
- [RabbitMQ Reliability](https://www.rabbitmq.com/reliability.html)
- [Service Bus](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
- [MassTransit](https://masstransit-project.com/)
- [MassTransit Docs](https://masstransit.io/documentation/concepts)
    - [Messages](https://masstransit.io/documentation/concepts/messages)
    - [Consumer](https://masstransit.io/documentation/concepts/consumers)
    - [Producer](https://masstransit.io/documentation/concepts/producers)
    - [Request/Response](https://masstransit.io/documentation/concepts/requests)
    - [Sagas](https://masstransit.io/documentation/patterns/saga)   
- Mass Transit Samples:
    - [Sample-DotNetConf](https://github.com/phatboyg/Sample-DotNetConf)
    - [Sample-JobConsumers](https://github.com/MassTransit/Sample-JobConsumers)
    - [Sample-Twitch](https://github.com/MassTransit/Sample-Twitch)
    - [SuuCat-Sample-MicroserviceApp](https://github.com/ebubekirdinc/SuuCat)
    - [Saga State Machine & MassTransit](https://github.com/bariscantanriverdi/SagaStateMachine-Async-Messaging/)

### MassTransit Demos

All demos require RabbitMQ to be installed and running (e.g. in container).

- **Asynchronous Communication**
    - Start MassTransit.WebApi (use TestController endpoints)
    - Start MassTransit.Worker
- **Competing Consumers / Load Balancing** - 
    - Start MassTransit.WebApi (use TestController endpoints)
    - Start two or more instances of MassTransit.Worker
- **Temporal Decoupling**
    - Start MassTransit.WebApi (use TestController endpoints)
    - Invoke API few times to send messages to the queue
    - Start MassTransit.Worker to consume messages from the queue


### MassTransit and Rabbit MQ Observability features

**MassTransit.WebApi** and **MassTransit.Worker** projects are configured to use some observability features like:

- Metrics via Prometheus and Grafana (in MassTransit.WebApi only)
    - Metrics are available at "http://localhost:5089/metrics" for MassTransit.WebApi and "http://localhost:15692/metrics" for RabbitMQ instance.
        - Following Grafana Dashboards are used:
            - https://grafana.com/grafana/dashboards/10991-rabbitmq-overview/
            - https://grafana.com/grafana/dashboards/17680-masstransit-messages-monitoring/
- Distributed Traces via Jeager
    - Traces are sent to OLTP endpoint at port 4317 in Jeager instance from MassTransit.WebApi and MassTransit.Worker.
- More information on Observability configuration in Mass Trasnit is available [here](https://masstransit.io/documentation/configuration/observability).


## Reliability patterns and techniques

- [IHttpClientFactory and Polly policies](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly)
- [Polly and HttpClientFactory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory)
- [Resiliency in gRPC](https://learn.microsoft.com/en-us/aspnet/core/grpc/retries?view=aspnetcore-7.0)
    - Example: Grpc.Client, Program.cs
- [Client-side Load Balancing](https://learn.microsoft.com/en-us/aspnet/core/grpc/loadbalancing?view=aspnetcore-7.0)
- [Connection Resiliency in Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency)
- [Polly](https://github.com/App-vNext/Polly)
- [Simmy](https://github.com/Polly-Contrib/Simmy)
- [Simmy, the monkey for making chaos](https://www.thepollyproject.org/2019/06/27/simmy-the-monkey-for-making-chaos/)
- [Rate limiting in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-7.0)

## Load & Stress Testing

- [Load test types](https://k6.io/docs/test-types/load-test-types/)
- Tools and Apps:
    - [Apache JMeter](https://jmeter.apache.org/)
    - [NBomber](https://nbomber.com/)
        - [Examples](https://github.com/PragmaticFlow/NBomber/tree/dev/examples/Demo)
        - [Tutorial](https://nbomber.com/docs/getting-started/hello_world_tutorial/)
        - Example: FaultyWebAPI.LoadTest, 
    - [k6](https://k6.io/docs/)
    - [Locust](https://locust.io/)

## Libraries & Tools to explore

- [CAP](https://github.com/dotnetcore/CAP)

## More Info
- [Asynchronous Messaging Primer](https://learn.microsoft.com/en-us/previous-versions/msp-n-p/dn589781(v=pandp.10))
- [Competing Consumers](https://learn.microsoft.com/en-us/azure/architecture/patterns/competing-consumers)