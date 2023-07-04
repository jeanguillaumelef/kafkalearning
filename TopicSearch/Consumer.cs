using System;
using System.Collections.Generic;
using System.Threading;
using AO.SCV.MessagingEngine.Events.Schemas;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Newtonsoft.Json;

namespace TopicSearch
{
    public class Consumer<TKeyType> : IDisposable
    {
        private readonly CachedSchemaRegistryClient cachedSchemaRegistryClient;
        private readonly IConsumer<TKeyType, MessagingResult> consumer;
        public Consumer(string brokerList, string schemaRegistryUrl, string schemarRegistryAuth, List<string> topics, string saslUsername, string saslPassword)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "csharp-consumer",
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = saslUsername,
                SaslPassword = saslPassword
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl,
                BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                BasicAuthUserInfo = schemarRegistryAuth,
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };

            cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            consumer = BuildConsumer(consumerConfig, cachedSchemaRegistryClient);

            consumer.Subscribe(topics);
        }

        private IConsumer<TKeyType, MessagingResult> BuildConsumer(ConsumerConfig consumerConfig, CachedSchemaRegistryClient schemaRegistryClient)
        {
            var consumerBuilder = new ConsumerBuilder<TKeyType, MessagingResult>(consumerConfig);
            consumerBuilder.SetKeyDeserializer(new AvroDeserializer<TKeyType>(schemaRegistryClient).AsSyncOverAsync());
            consumerBuilder.SetValueDeserializer(new AvroDeserializer<MessagingResult>(schemaRegistryClient).AsSyncOverAsync());
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
                        ConsumeResult<TKeyType, MessagingResult> consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult.IsPartitionEOF)
                        {
                            Console.WriteLine(
                                $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                        }
                        else
                        {
                            MessagingResult message = consumeResult.Message.Value;
                            if (MessageFilter.IsMessageValid(message))
                            {
                                Console.WriteLine("Message found");
                                Console.WriteLine(JsonConvert.SerializeObject(message));
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

        private Message<TKeyType, MessagingResult> GetMessageFromConsumeResult(ConsumeResult<TKeyType, MessagingResult> consumeResult)
        {
            Message<TKeyType, MessagingResult> result = null;

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
