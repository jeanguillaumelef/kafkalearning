using System;
using System.Collections.Generic;
using System.Threading;

namespace KafkaConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var brokerList = "localhost:9092";
            var topics = new List<string> { "MySimpleTopic" };

            Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            IMessageProcessor processor = new MessageProcessor();

            var consumer = new Consumer(brokerList, topics, processor);
            consumer.Run_Consume(cts.Token);
        }
    }
}
