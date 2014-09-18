DynatraceToSplunk
=================

Reads dynaTrace dashboards using REST interface and ships the data to Splunk 

To run you'll need a config.txt in the same directory as the executable that contains the following information:

SplunkServer	192.168.1.1
SplunkAdmin	admin
SplunkPass	password
SplunkIndex	main
SplunkHost	ExampleHost
DynaTraceServer	192.168.1.1
DynaTraceUser	admin
DynaTracePass	admin
Dashboard	easyTravel PurePaths



It's pretty picky when it comes to reading this file. The values on the left need to be spelled just like they are above.

There's also a TAB between the names of the variables and the values themselves. The program is splitting on tabs to read in 
those strings properly. This functionality is just a placeholder.


At the moment it only can read in one dashboard at a time, but you can put multiple dashlets in that one dashboard. 

During trials it's read over 5000 events to Splunk. I'm still not sure on the limit on the amount of data that can be pushed.
