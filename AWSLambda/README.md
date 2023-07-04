## Requirements

To understand this documentation it is needed to know:  
-what is an AWS VPC  
-what is an AWS Lambda  
-what is Kafka  

This project gives an example of a Kafka connector and a AWS Lambda.
You can find more explanation on https://docs.confluent.io/current/connect/kafka-connect-aws-lambda/index.html

## Introduction

Let's say we have a Kafka topic and an AWS Lambda function. To trigger the Lambda function, we need something which translate a Kafka message into an AWS event. To do this translation we can use an AWS Lambda Sink Connector.  

## Basic functionment
The connector poll the topic to get a batch of message, translate into an AWS event and returns it to the AWS Lambda.
The connector can be configured to call the lambda function synchronously and asynchronously.

### Asynchronous  

  

### Synchronous  
  
When called synchronously the Kafka connector can optionally log the response from a lambda in a kafka topic. The connector can optionally log errors to an error topic, the error topic is configured through "aws.lambda.error.*"  

  
## Steps
1. Setup the AWS Lambda service and the VPC
2. Setup the Kafka Connector
  
## 1 - Setup the AWS Lambda service synchronously

To setup a lambda service we are going to use serverless framework(https://serverless.com/framework/docs/ ).  
Here an example of a serverless file:  
  
```
	service: awslambda ##name of the service hosted on AWS

	provider:
	  name: aws ##provider name
	  runtime: dotnetcore2.1
	  stage: ${opt:stage, 'dev'}
	  region: eu-west-1
	  iamRoleStatements: ##used to give permission to your service
		- Effect: Allow
		  Action:
			- lambda:InvokeFunction
		  Resource: "*"
	  vpc: ##configuration of the vpc where the service will sit
		securityGroupIds:
		  - ${file(../serverless.env.yml):${opt:stage}.SECURITY_GROUP}
		subnetIds:
		  - ${file(../serverless.env.yml):${opt:stage}.SUBNET-ONE}
		  - ${file(../serverless.env.yml):${opt:stage}.SUBNET-TWO}
		  - ${file(../serverless.env.yml):${opt:stage}.SUBNET-THREE}
	package:
	  artifact: ../../dist/awslambda.zip ##name of the output package after building th solution

	functions:
	  tagRequest: ## lambda function name
		handler: awslambda::aawslambda.FunctionService::FunctionServiceHandler
```

The serverless file will given to aws through this command.
```
serverless config credentials --provider aws --key ${AWS_ACCESS_KEY_ID} --secret ${AWS_SECRET_ACCESS_KEY} --stage $env
```

This command can be found in the deploy.sh script which will be call through the bitbucket pipeline process.


## 2 - Setup the Kafka Connector

Here an example of a Kafka Connector configuration file:  
  
```
{
    "name": "staging-awslambda-requesttagging",
    "config": {
        "connector.class": "io.confluent.connect.aws.lambda.AwsLambdaSinkConnector",
        "tasks.max": "1",
        "topics": "socket_visitorjourney_visitor_page_visit", ##Topic to consume from
        "aws.lambda.function.name": "arn:aws:lambda:eu-west-1:420021094716:function:CP-ClickstreamTagging-staging-tagRequest", ##ARN of the AWS Lambda to trigger
        "aws.lambda.invocation.type": "sync",
        "aws.lambda.batch.size": "1",
        "behavior.on.error": "fail",
        "key.converter": "org.apache.kafka.connect.storage.StringConverter",
        "value.converter": "org.apache.kafka.connect.storage.StringConverter",
        "value.converter.schemas.enable": "false",
        "confluent.topic.bootstrap.servers": "SASL_SSL://pkc-43n2o.eu-west-1.aws.confluent.cloud:9092",
        "confluent.topic.replication.factor": "1",
        "confluent.topic.ssl.endpoint.identification.algorithm": "https",
        "confluent.topic.sasl.mechanism": "PLAIN",
        "confluent.topic.request.timeout.ms": 20000,
        "confluent.topic.retry.backoff.ms": 500,
        "confluent.topic.sasl.jaas.config": "org.apache.kafka.common.security.plain.PlainLoginModule required username=\"USERNAME\" password=\"PASSWORD\";",
        "confluent.topic.security.protocol": "SASL_SSL",
        "aws.lambda.response.topic": "clickstream_tagging_request_tagged", ##Topic to produce to
        "aws.lambda.response.bootstrap.servers": "SASL_SSL://pkc-43n2o.eu-west-1.aws.confluent.cloud:9092",
        "aws.lambda.response.replication.factor": "1",
        "aws.lambda.response.ssl.endpoint.identification.algorithm": "https",
        "aws.lambda.response.sasl.mechanism": "PLAIN",
        "aws.lambda.response.request.timeout.ms": 20000,
        "aws.lambda.response.retry.backoff.ms": 500,
        "aws.lambda.response.sasl.jaas.config": "org.apache.kafka.common.security.plain.PlainLoginModule required username=\"USERNAME\" password=\"PASSWORD\";",
        "aws.lambda.response.security.protocol": "SASL_SSL"
    }
}
```
