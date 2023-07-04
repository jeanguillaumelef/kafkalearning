
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Newtonsoft.Json;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaProducer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var brokerList = "localhost:9092";
            var topicName = "testTopic2Part";
            //var schemaRegistryUrl = "";

            //var schemaRegistryConf = new SchemaRegistryConfig { Url = schemaRegistryUrl };
            //var schameRegistry = new CachedSchemaRegistryClient(schemaRegistryConf);

            var config = new ProducerConfig
            {
                BootstrapServers = brokerList
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //SaslMechanism = SaslMechanism.Plain,
                //SaslUsername = "",
                //SaslPassword = "",
            };
            string theObject = "{PropertyKey: propertyValue}";

            using (var producer = new ProducerBuilder<string, string>(config).Build()
            ) //new AvroSerializer<Test>(schameRegistry)).Build())
            {

                do
                {


                    try
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, string> { Key = null, Value = theObject });

                        Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, Test> e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                } while (true);
            }

            //Console.ReadKey();


        }
    }

    class Test
    {
        public string A { get; set; }
        public int B { get; set; }
    }
}