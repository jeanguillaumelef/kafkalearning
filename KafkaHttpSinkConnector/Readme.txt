Need to build the customer docker image here: ao.kafkalearning\AvroSchemaManagement\dockerfile
docker build . -t httpsync:1.0.0

Need to build the customer docker image here: ao.kafkalearning\KafkaHttpSinkConnector\apiServer
docker build . -t apiserver:1.0.0
	
docker-compose up -d --build


Command to run in connector (need api for apiserver for http.api.url, run docker inspect apiserver and look at bottom of text for ip address):

 curl -X POST -H "Content-Type: application/json" --data '{"name":"test",  "config": {
     "topics": "brandnewtopic",
     "tasks.max": "1",
     "connector.class": "io.confluent.connect.http.HttpSinkConnector",
     "http.api.url": "http://172.18.0.2:8055/api/messages/${topic}/${key}",
     "value.converter": "org.apache.kafka.connect.storage.StringConverter",
     "confluent.topic.bootstrap.servers": "broker:29092",
     "confluent.topic.replication.factor": "1",
     "reporter.bootstrap.servers": "broker:29092",
     "reporter.result.topic.name": "success-responses",
     "reporter.result.topic.replication.factor": "1",
     "reporter.error.topic.name":"error-responses",
     "reporter.error.topic.replication.factor":"1"
   }}' localhost:8083/connectors/
   
  
 To get status:
  
  curl -X GET localhost:8083/connectors/test/status
  
  
  
