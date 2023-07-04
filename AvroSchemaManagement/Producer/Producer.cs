using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace Producer
{
    public class Producer<TKeyType, TValueType>
    {
        private readonly IProducer<TKeyType, TValueType> producer;
        private readonly CachedSchemaRegistryClient cachedSchemaRegistryClient;
        
        public Producer(ProducerConfig producerConfig, CachedSchemaRegistryClient cachedSchemaRegistryClient)
        {
            this.cachedSchemaRegistryClient = cachedSchemaRegistryClient;

            var producerBuilder = new ProducerBuilder<TKeyType, TValueType>(producerConfig);
            producerBuilder.SetKeySerializer(new AvroSerializer<TKeyType>(cachedSchemaRegistryClient));
            producerBuilder.SetValueSerializer(new AvroSerializer<TValueType>(cachedSchemaRegistryClient));
            producer = producerBuilder.Build();
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

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                producer.Flush(TimeSpan.FromSeconds(5));
                producer.Dispose();
                cachedSchemaRegistryClient.Dispose();
            }
        }
    }
}
