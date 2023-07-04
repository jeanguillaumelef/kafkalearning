## Introduction

Sometimes your consumer indexes needs to be changed.
Two cases comes to my mind when we want to change the partition indexes.

### A lot of messages are in the partition and you don't need to read them

Let's say that you start a new consumer and your put the setting AutoOffsetReset to Earliest.
This means when you consumer first start to consume the topic it will put the index on the earliest message. 
The side effect of this is we will have to read all the messages from the start of the queue before catching up to the latest messages.
In introspection AutoOffsetReset should have been set to Latest but now we have this index at the start when we wanted it at the end.
