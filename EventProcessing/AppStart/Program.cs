using System.Threading;
using Communication;
using Confluent.Kafka;
using Ninject;

namespace AppStart
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel( new ServiceModule()))
            {

                var consumer = kernel.Get<IKafkaConsumer<Null, string>>();
                var eventProcessor = kernel.Get<IEventProcessor>();
                consumer.StartConsuming(new CancellationToken(), eventProcessor.ProcessEvent);
            }
        }
    }
}
