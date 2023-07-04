using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace Communication
{
    public class KafkaProducer<TKeyType, TValueType> : IDisposable, IKafkaProducer<TKeyType, TValueType>
    {
        private readonly IProducer<TKeyType, TValueType> producer;
        private readonly CachedSchemaRegistryClient schemaRegistry;

        //TODO too much exception unsafe method in the constructor, should build a factory
        public KafkaProducer(string brokerList, string schemaRegistryUrl)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = brokerList };
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl
            };

            //TODO should extract this into a factory
            schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            producer = BuildProducer(producerConfig, schemaRegistry);
        }

        private IProducer<TKeyType, TValueType> BuildProducer(ProducerConfig producerConfig, CachedSchemaRegistryClient cachedSchemaRegistryClient)
        {
            var producerBuilder = new ProducerBuilder<TKeyType, TValueType>(producerConfig);
            producerBuilder.SetKeySerializer(new AvroSerializer<TKeyType>(cachedSchemaRegistryClient));
            producerBuilder.SetValueSerializer(new AvroSerializer<TValueType>(cachedSchemaRegistryClient));
            return producerBuilder.Build();
        }

        public async Task PublishMessage(string topicName, TKeyType key, TValueType value)
        {
            var msg = new Message<TKeyType, TValueType> { Key = key, Value = value };
            try
            {
                await producer.ProduceAsync(topicName, msg);
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

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                producer.Flush(TimeSpan.FromSeconds(5));
                producer.Dispose();
                schemaRegistry.Dispose();
            }
        }
    }
}
