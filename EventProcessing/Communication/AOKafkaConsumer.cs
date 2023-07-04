using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace Communication
{
    public class AOKafkaConsumer<TKeyType, TValueType> : IDisposable, IAOKafkaConsumer<TKeyType, TValueType>
    {
        private readonly IConsumer<TKeyType, TValueType> _consumer;

        public AOKafkaConsumer(string brokerList, List<string> topics)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "test",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            _consumer = new ConsumerBuilder<TKeyType, TValueType>(consumerConfig).Build();
            _consumer.Subscribe(topics);
        }

        public void StartConsuming(CancellationToken cancellationToken, Action<TValueType> processEvent)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ProcessEvent(cancellationToken, processEvent);
            }
        }

        private void ProcessEvent(CancellationToken cancellationToken, Action<TValueType> processEvent)
        {
            try
            {
                ConsumeResult<TKeyType, TValueType> consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult.IsPartitionEOF)
                {
                    Console.WriteLine(
                        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                }
                else
                {
                    processEvent(consumeResult.Message.Value);
                    _consumer.Commit(consumeResult);
                }
            }
            catch (ConsumeException e)
            {
                HandleKafkaException(e);
            }
        }

        private void HandleKafkaException(KafkaException e)
        {
            Console.WriteLine($"Commit error: {e.Error.Reason}");
            _consumer.Close();
        }

        public void Dispose()
        {
            _consumer.Close();
        }
    }

}
