namespace BusinessLogic
{
    public interface IDataProcessor
    {
        DataResponse Process(EventData data);
    }
}