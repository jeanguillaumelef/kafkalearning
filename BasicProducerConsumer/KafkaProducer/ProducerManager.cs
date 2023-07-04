using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaProducer
{
    public class ProducerManager
    {
        private readonly IProducer<string, string> Producer;
        private readonly string TopicName;

        public ProducerManager(string brokerList, string topicName)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //SaslMechanism = SaslMechanism.Plain,
                //SaslUsername = "",
                //SaslPassword = "",
            };

            TopicName = topicName;

            var producerBuilder = new ProducerBuilder<string, string>(config);
            Producer = producerBuilder.Build();
        }

        public async Task PublishMessage(string key, string value)
        {
            var msg = new Message<string, string> { Key = key, Value = value };

            try
            {
                await Producer.ProduceAsync(TopicName, msg);
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                Producer.Flush(TimeSpan.FromSeconds(5));
                Producer.Dispose();
            }
        }
    }
}