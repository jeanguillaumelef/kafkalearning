using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;

namespace KafkaConsumer
{
    public class Consumer
    {
        private readonly ConsumerConfig ConsumerConfig;
        private readonly List<string> Topics;
        private IMessageProcessor MessageProcessor;

        public Consumer(string brokerList, List<string> topics, IMessageProcessor messageProcessor)
        {
            ConsumerConfig = SetConfig(brokerList);
            Topics = topics;
            MessageProcessor = messageProcessor;
        }

        public ConsumerConfig SetConfig(string brokerList)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "local-dev",
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //SaslMechanism = SaslMechanism.Plain,
                //SaslUsername = "",
                //SaslPassword = ""

            };

            return config;
        }

        public void Run_Consume(CancellationToken cancellationToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetPartitionsAssignedHandler(PartitionAssignmentHandler)
                .SetPartitionsRevokedHandler(PartitionsRevokedHandler)
                .Build())
            {
                consumer.Subscribe(Topics);
                try
                {
                    StartConsumptionLoop(consumer, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }

        public void StartConsumptionLoop(IConsumer<Ignore, string> consumer, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine(
                            $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                        continue;
                    }

                    Console.Write($"Received message at {consumeResult.TopicPartitionOffset}");
                    MessageProcessor.processMessage(consumeResult.Message.Value);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
            }
        }

        private void PartitionAssignmentHandler(IConsumer<Ignore, string> c, List<TopicPartition> partitions)
        {
            Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
        }

        private void PartitionsRevokedHandler(IConsumer<Ignore, string> c, List<TopicPartitionOffset> partitions)
        {
            Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
        }
    }
}
