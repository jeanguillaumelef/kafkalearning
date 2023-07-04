using System;
using System.Collections.Generic;
using System.Threading;

namespace TopicSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = ""; 
            string schemaRegistryUrl = ""; 
            string schemarRegistryAuth = ""; 
            List< string > topics = new List<string>(){ "" };
            string saslUsername = "";
            string saslPassword = "";

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            var consumer = new Consumer<string>(brokerList, schemaRegistryUrl, schemarRegistryAuth, topics, saslUsername, saslPassword);
            consumer.Consume(cts.Token);


        }
    }
}
