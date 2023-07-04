using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;

namespace ConsumeOnly
{
    public class Function
    {
        public class FunctionController
        {
            public void FunctionServiceHandler(List<Request> inputs, ILambdaContext context)
            {
                foreach (var input in inputs)
                {
                    Console.WriteLine($"Partition:{input.Payload.Partition}, Offset:{input.Payload.Offset}");
                    Console.WriteLine($"Key:{input.Payload.Key}");
                    Console.WriteLine($"Value:{input.Payload.Value}");
                }
            }
        }

        public class Request
        {
            public RequestPayload Payload { get; set; }
        }

        public class RequestPayload
        {
            public long Timestamp { get; set; }
            public string Topic { get; set; }
            public int Partition { get; set; }
            public int Offset { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}