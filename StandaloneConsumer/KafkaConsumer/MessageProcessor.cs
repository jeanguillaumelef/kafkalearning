using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaConsumer
{
    public class MessageProcessor: IMessageProcessor
    {
        public void processMessage(string value)
        {
            Console.WriteLine($" MessageContent: {value}");
        }
    }
}
