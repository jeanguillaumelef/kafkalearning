using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace Producer
{
    public class ProducerFactory<TKeyType, TValueType> : IProducerFactory<TKeyType, TValueType>
    {
        public Producer<TKeyType, TValueType> CreateProducer(string brokerList, string schemaRegistryUrl)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = brokerList };
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl
            };

            var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            return new Producer<TKeyType,TValueType>(producerConfig, schemaRegistry);
        }
    }
}