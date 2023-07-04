using aoevent;

namespace Communication.Producers
{
    public interface IAOProducer
    {
        void Produce(AOEvent resultEvent);
    }
}