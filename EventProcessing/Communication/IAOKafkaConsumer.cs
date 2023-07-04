using System;
using System.Threading;

namespace Communication
{
    public interface IAOKafkaConsumer<TKeyType, TValueType>
    {
        void StartConsuming(CancellationToken cancellationToken, Action<TValueType> processEvent);
        void Dispose();
    }
}