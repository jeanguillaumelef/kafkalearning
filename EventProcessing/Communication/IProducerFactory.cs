namespace Communication
{
    public interface IProducerFactory<TKeyType, TValueType>
    {
        IAOKafkaProducer<TKeyType, TValueType> Build(string brokerList);
    }
}