using System;
using Confluent.Kafka;
using Message;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = "localhost:9092";
            string schemaRegistryUrl = "http://localhost:8081";
            string topicName = "ab2";

            var message = new AB
            {
                A = 1,
                B = 2
            };

            IProducerFactory<string, AB> producerFactory = new ProducerFactory<string, AB>();
            
            var producer = producerFactory.CreateProducer(brokerList, schemaRegistryUrl);
            producer.PublishMessage(topicName, "a", message).Wait();
            producer.Dispose();
        }
    }
}
