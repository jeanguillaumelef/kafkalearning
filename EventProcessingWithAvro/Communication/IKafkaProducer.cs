using System.Threading.Tasks;

namespace Communication
{
    public interface IKafkaProducer<TKeyType, TValueType>
    {
        Task PublishMessage(string topicName, TKeyType key, TValueType value);
        void Dispose();
    }
}