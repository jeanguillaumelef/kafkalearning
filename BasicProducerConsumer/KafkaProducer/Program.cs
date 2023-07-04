
using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaProducer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var brokerList = "localhost:9092";
            var topicName = "MySimpleTopic";

            ProducerManager producerManager = new ProducerManager(brokerList, topicName);

            do
            {
                Console.WriteLine("publishing message");
                producerManager.PublishMessage("my key", "my important message").Wait();

                Console.ReadKey();
            } while (true);

        }
    }
}