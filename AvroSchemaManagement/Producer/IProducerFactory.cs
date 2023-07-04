namespace Producer
{
    public interface IProducerFactory<TKeyType, TValueType>
    {
        Producer<TKeyType, TValueType> CreateProducer(string brokerList, string schemaRegistryUrl);
    }
}