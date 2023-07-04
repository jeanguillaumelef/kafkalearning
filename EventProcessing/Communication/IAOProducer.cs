using aoevent;

namespace Communication
{
    public interface IAOProducer
    {
        void Produce(AOEvent resultEvent);
    }
}