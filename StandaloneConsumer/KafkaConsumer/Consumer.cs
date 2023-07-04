using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;

namespace KafkaConsumer
{
    public class Consumer
    {
        private readonly ConsumerConfig ConsumerConfig;
        private readonly string Topic;
        private IMessageProcessor MessageProcessor;

        public Consumer(string brokerList, string topic, IMessageProcessor messageProcessor)
        {
            ConsumerConfig = SetConfig(brokerList);
            Topic = topic;
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
            };

            return config;
        }

        public void Run_Consume(CancellationToken cancellationToken)
        {
            //Use the AdminClient to retrieve partition details for the given topic
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = ConsumerConfig.BootstrapServers }).Build())
            {
                var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
                var topic = meta.Topics.SingleOrDefault(t => t.Topic == Topic);
                var topicPartitionsMetadata = topic.Partitions;

                if (topicPartitionsMetadata != null)
                {
                    using (var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig)
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .SetPartitionsAssignedHandler(PartitionAssignmentHandler)
                        .SetPartitionsRevokedHandler(PartitionsRevokedHandler)
                        .Build())
                    {
                        List<TopicPartition> partitions = topicPartitionsMetadata.Select(metadata =>
                            new TopicPartition(Topic, new Partition(metadata.PartitionId))).ToList();

                        consumer.Assign(partitions); //Assign all partitions to the consumer

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
