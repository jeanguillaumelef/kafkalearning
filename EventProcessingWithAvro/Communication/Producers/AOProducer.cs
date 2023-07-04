using aoevent;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Communication.Producers
{
    public class AOProducer : IAOProducer
    {
        private readonly IKafkaProducer<Null, AOEvent> _aoKafkaProducer;
        private readonly string _topic;

        public AOProducer(IKafkaProducer<Null, AOEvent> aoKafkaProducer, string topic)
        {
            _aoKafkaProducer = aoKafkaProducer;
            _topic = topic;
        }

        public void Produce(AOEvent resultEvent)
        {
            _aoKafkaProducer.PublishMessage(_topic, null, resultEvent);
        }
    }
}
