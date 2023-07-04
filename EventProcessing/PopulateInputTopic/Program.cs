using System;
using aoevent;
using aoevent.metadata;
using BusinessLogic;
using Communication;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace PopulateInputTopic
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = "localhost:9092";
            string topic = "Input";
            var producerConfig = new ProducerConfig { BootstrapServers = brokerList };
            var aoKafkaProducer = new AOKafkaProducer<Null, string>(producerConfig, topic);
            var aoProducer = new AOProducer(aoKafkaProducer);

            while (true)
            {
                Console.WriteLine("push key to produce");
                Console.ReadKey();

                MetadataIds metadataIds = new MetadataIds();
                Description description = new Description(metadataIds, "test");
                AOMetadata aoMetadata = new AOMetadata(description, new Administration());
                EventData eventData = new EventData
                {
                    Message =
                        $"CorrelationId={aoMetadata.Description.MetadataIds.CorrelationId} "
                        + $"CausationId={aoMetadata.Description.MetadataIds.CausationId} "
                        + $"Id={aoMetadata.Description.MetadataIds.EventId}"
                };
                var rawEventData = JsonConvert.SerializeObject(eventData);
                AOEvent aoEvent = new AOEvent(aoMetadata, rawEventData);

                aoProducer.Produce(aoEvent);
            }
        }
    }
}
