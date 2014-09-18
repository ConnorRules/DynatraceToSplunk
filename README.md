DynatraceToSplunk
=================

Reads dynaTrace dashboards using REST interface and ships the data to Splunk 

To run you'll need a config.txt in the same directory as the executable. There's an example in the bin of such a config.txt file. 

It's pretty picky when it comes to reading this file. The values on the left need to be spelled just like they are in the example.

There's also a TAB between the names of the variables and the values themselves. The program is splitting on tabs to read in 
those strings properly. This functionality is just a placeholder.


At the moment it only can read in one dashboard at a time, but you can put multiple dashlets in that one dashboard. 

During trials it's read over 5000 events to Splunk. I'm still not sure on the limit on the amount of data that can be pushed.
