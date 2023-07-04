## Introduction

To understand this documentation it is needed to know:  
-what is a message schema  
-what is a schema registry  
-what is Kafka  

This project demonstrate :  
-how the schema registry is working  
-how the compatibility policy affects schema evolution  

## Start up
run "docker-compose up -d --build" in the solution folder to setup the environment required to have a kafka broker working with avro schema registry

## Backward policy cheat sheet

### Publisher
| Action 				| IsCompatible | Note |
| --- | --- | --- |
| Add field 			| No  | |
| Add optional field 	| Yes | Update schema |
| Delete field 			| Yes | Update schema |
| Delete optional field | Yes | Update schema |

### Subscriber
| Action 				| IsCompatible | Note |
| --- | --- | --- |
| Add field 			| No  | |
| Add optional field 	| Yes | Fill it with default value |
| Delete field 			| Yes | Ignore missing field |
| Delete optional field | Yes | Ignore missing field |