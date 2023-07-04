using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Newtonsoft.Json;

namespace Consumer
{
    public class Consumer<TKeyType, TValueType> : IDisposable
    {
        private readonly CachedSchemaRegistryClient cachedSchemaRegistryClient;
        private readonly IConsumer<TKeyType, TValueType> consumer;
        public Consumer(string brokerList, string schemaRegistryUrl, List<string> topics)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "csharp-consumer",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl
            };

            cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            consumer = BuildConsumer(consumerConfig, cachedSchemaRegistryClient);

            consumer.Subscribe(topics);
        }

        private IConsumer<TKeyType, TValueType> BuildConsumer(ConsumerConfig consumerConfig, CachedSchemaRegistryClient schemaRegistryClient)
        {
            var consumerBuilder = new ConsumerBuilder<TKeyType, TValueType>(consumerConfig);
            consumerBuilder.SetKeyDeserializer(new AvroDeserializer<TKeyType>(schemaRegistryClient).AsSyncOverAsync());
            consumerBuilder.SetValueDeserializer(new AvroDeserializer<TValueType>(schemaRegistryClient).AsSyncOverAsync());
            consumerBuilder.SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"));
            consumerBuilder.SetPartitionsAssignedHandler((c, partitions) =>
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
            });
            consumerBuilder.SetPartitionsRevokedHandler((c, partitions) =>
            {
                Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
            });

            return consumerBuilder.Build();
        }

        public void Consume(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        ConsumeResult<TKeyType, TValueType> consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult.IsPartitionEOF)
                        {
                            Console.WriteLine(
                                $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                        }
                        else
                        {
                            TValueType message = consumeResult.Message.Value;
                            Console.WriteLine(JsonConvert.SerializeObject(message));

                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
        }

        private Message<TKeyType, TValueType> GetMessageFromConsumeResult(ConsumeResult<TKeyType, TValueType> consumeResult)
        {
            Message<TKeyType, TValueType> result = null;

            if (consumeResult.IsPartitionEOF)
            {
                Console.WriteLine(
                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
            }
            else
            {
                try
                {
                   result = consumeResult.Message;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    consumer.Commit(consumeResult);
                }
                catch (KafkaException e)
                {
                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                }
            }

            return result;
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
                consumer.Close();
                consumer.Dispose();
                cachedSchemaRegistryClient.Dispose();
            }
        }
    }


}
