using System;
using Confluent.Kafka;

namespace Communication
{
    public class AOKafkaProducer<TKeyType, TValueType> : IAOKafkaProducer<TKeyType, TValueType>
    {
        private readonly string _topic;
        private readonly IProducer<TKeyType, TValueType> _producer;
        public AOKafkaProducer(ProducerConfig config, string topic)
        {
            _producer = new ProducerBuilder<TKeyType, TValueType>(config).Build();
            _topic = topic;
        }

        public bool TrySend(TKeyType key, TValueType value)
        {
            var success = true;
            try
            {
                var kafkaMessage = new Message<TKeyType, TValueType> { Key = key, Value = value };

                _producer.Produce(_topic, kafkaMessage);
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                success = false;
            }

            return success;
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
