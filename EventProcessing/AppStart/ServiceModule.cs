using System;
using System.Collections.Generic;
using System.Text;
using Event;
using BusinessLogic;
using Communication;
using Confluent.Kafka;
using Ninject.Modules;

namespace AppStart
{
    public class ServiceModule: NinjectModule
    {
        private const string BrokerList = "localhost:9092";
        private const string TopicToConsumeFrom = "Input";
        private const string TopicToProduceTo = "OutPut";

        public override void Load()
        {
            ProducerConfig producerConfig = new ProducerConfig { BootstrapServers = BrokerList };
            
            Bind<IDataProcessor>()
                .To<DataProcessor>();            
            
            Bind<IKafkaConsumer<Null, string>>()
                .ToMethod(consumer => new KafkaConsumer<Null, string>(BrokerList, new List<string> { TopicToConsumeFrom }));            
            
            Bind<IKafkaProducer<Null, string>>()
                .ToMethod(producer => new KafkaProducer<Null, string>(producerConfig, TopicToProduceTo));            
            
            Bind<IProducer>()
                .To<Producer>();
            
            Bind<IEventProcessor>()
                .To<EventProcessor>(); 

            Bind<IAOMetaDataValidator>()
                .To<AOMetaDataValidator>();
        }
    }
}
