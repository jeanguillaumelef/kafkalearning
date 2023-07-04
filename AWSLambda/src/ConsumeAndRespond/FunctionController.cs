using System.Collections.Generic;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ConsumeAndRespond
{
    public class FunctionController
    {
        public List<Output> FunctionServiceHandler(List<Request> inputs, ILambdaContext context)
        {
            List<Output> result = null;
            return result;
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

    public class Output
    {
        public OutputPayload OutputPayload { get; set; }
    }

    public class OutputPayload
    {
        public long Timestamp { get; set; }
        public string Topic { get; set; }
        public int Partition { get; set; }
        public int Offset { get; set; }
        public string Result { get; set; }
    }
}