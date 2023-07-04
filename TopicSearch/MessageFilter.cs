using AO.SCV.MessagingEngine.Events.Schemas;

namespace TopicSearch
{
    public static class MessageFilter
    {
        //put your filtering logic in there
        public static bool IsMessageValid(MessagingResult message)
        {

            return message.visitorId == "6306ee29-0292-4922-831f-be085d31a19a";
        }
    }
}
