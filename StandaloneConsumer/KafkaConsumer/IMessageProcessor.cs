namespace KafkaConsumer
{
    public interface IMessageProcessor
    {
        void processMessage(string value);
    }
}