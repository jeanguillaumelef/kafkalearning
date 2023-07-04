namespace Communication
{
    public interface IAOKafkaProducer<TKeyType, TValueType>
    {
        bool TrySend(TKeyType key, TValueType value);
        void Dispose();
    }
}