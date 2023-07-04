
namespace BusinessLogic
{
    public class DataProcessor : IDataProcessor
    {
        public DataResponse Process(EventData data)
        {
            var response = new DataResponse
            {
                Result = "Result" + data.Message
            };

            return response;
        }
    }
}
