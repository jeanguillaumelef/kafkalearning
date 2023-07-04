using System.Collections.Generic;
using aoevent;
using aoevent.metadata;
using BusinessLogic;
using Newtonsoft.Json;

namespace Communication
{
    public class EventProcessor: IEventProcessor
    {
        private const string ApplicationName = "thisApplication";
        private readonly IAOMetaDataValidator _aoMetaDataValidator;
        private readonly IDataProcessor _dataProcessor;
        private readonly IAOProducer _aoEventProducer;

        public EventProcessor(IAOMetaDataValidator aoMetaDataValidator, IDataProcessor dataProcessor, IAOProducer aoProducer)
        {
            _aoMetaDataValidator = aoMetaDataValidator;
            _dataProcessor = dataProcessor;
            _aoEventProducer = aoProducer;
        }

        public void ProcessEvent(string aoEvent)
        {
            var aoEventObject = JsonConvert.DeserializeObject<AOEvent>(aoEvent);
            bool isValid = _aoMetaDataValidator.DescriptionValidation(aoEventObject.AoMetadata.Description);

            if (isValid)
            {
                var tags = new Dictionary<string, string>();
                var result = StartProcessing(aoEventObject.Data);

                AOMetadata resultAoMetadata = new AOMetadata(aoEventObject.AoMetadata, ApplicationName, tags);
                AOEvent resultEvent = new AOEvent(resultAoMetadata, result);
                _aoEventProducer.Produce(resultEvent);
            }
        }

        private string StartProcessing(string rawData)
        {
            var data = JsonConvert.DeserializeObject<EventData>(rawData);

            DataResponse result = _dataProcessor.Process(data);

            var serializedResult = JsonConvert.SerializeObject(result);
            return serializedResult;
        }
    }
}
