using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Message;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = "localhost:9092";
            string schemaRegistryUrl = "http://localhost:8081";
            List<string> topicNames = new List<string>{ "MyTopic" };

            var consumer = new Consumer<Null, AB>(brokerList, schemaRegistryUrl, topicNames);
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
                consumer.Dispose();
            };

            //Force to run on another context, this is to avoid deadlocks (look at syncoverasync documentation)
            //Task.Run(() =>
            //{
            //    consumer.Consume(cts.Token);
            //}, cts.Token).ConfigureAwait(false);
            

            consumer.Consume(cts.Token);
        }
    }
}
