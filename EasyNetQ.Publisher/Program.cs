using EasyNetQ.Messages;
using EasyNetQ;

Console.WriteLine("Hello, EasyNetQ Subscriber!");

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
    var input = String.Empty;
    Console.WriteLine("Enter a message. 'Quit' to quit.");
    while ((input = Console.ReadLine()) != "Quit")
    {
        bus.PubSub.Publish(new TextMessage { Text = input });
        Console.WriteLine("Message published!");
    }
}