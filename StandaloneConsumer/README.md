# Standalone Consumer

This example shows a Consumer consuming all partitions for a given Topic.
Running multiple instances of this Consumer will result in each instance consuming all messages from the Topic.
This should really only be considered if multiple Consumer Groups is unsuitable.

NOTE - standalone consumers will consume from the given partitions and no rebalancing will take place. If your partitions change, you will need to implement a mechanism for updating the assigned Consumer partitions or restart all instances of your consumer application.