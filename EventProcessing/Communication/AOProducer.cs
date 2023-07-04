using aoevent;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Communication
{
    public class AOProducer : IAOProducer
    {
        private readonly IAOKafkaProducer<Null, string> _aoKafkaProducer;

        public AOProducer(IAOKafkaProducer<Null, string> aoKafkaProducer)
        {
            _aoKafkaProducer = aoKafkaProducer;
        }

        public void Produce(AOEvent resultEvent)
        {
            var serializedEvent = JsonConvert.SerializeObject(resultEvent);
            _aoKafkaProducer.TrySend(null, serializedEvent);
        }
    }
}
