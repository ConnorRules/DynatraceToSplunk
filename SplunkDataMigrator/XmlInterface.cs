using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;

namespace SplunkDataMigrator
{
    class XmlInterface
    {

        public System.Collections.Generic.IEnumerable<string> Read(string xmlString)
        {
            StringBuilder output = new StringBuilder();
            System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
            string systemProfile = "";
            string currentMeasure = "";
            string currentChart = "";   

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            ///////////Setting the system profile
                            if (reader.Name == "source")
                            {
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    if (reader.Name == "name") systemProfile = reader.Value.Replace(" ", "_");
                                }
                            }
                            

                            ////////////Reading Purepaths
                            if (reader.Name == "purepath" && reader.HasAttributes)
                            {
                                string message = "";
                                message += "\"SystemProfile\":\"" + systemProfile + "\", ";

                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    //Remove breakdown attribute due to programmer laziness
                                    //(The format is awful)
                                    if (reader.Name == "breakdown") continue;

                                    //Convert the date time to a splunkier format
                                    //This does not yet replace the time of the event
                                    if (reader.Name == "start")
                                    {
                                        DateTime dt = new DateTime();
                                        const string FORMAT = "ddd MMM dd HH:mm:ss  yyyy";

                                        if (DateTime.TryParseExact(reader.Value.ToString().Remove(20,3), FORMAT, CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                                        {
                                            dt = dt.AddHours(4);
                                            string date = dt.ToString("MMM dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                                            message = "\"time\":\"" + date + "\", " + message;
                                            //message = "\"time\":\"" + DateTime.UtcNow + "\", " + message;
                                            continue;
                                        }
                                    }

                                    //Create the message for this attribute
                                    message += "\"" + reader.Name.Replace(" ", "_") + "\":\"" + reader.Value.Replace(" ", "_") + "\", ";
                                }
                                yield return message;
                            }

                            ///////////////Reading Methods
                            if (reader.Name == "method" && reader.HasAttributes)
                            {
                                string message = "";
                                message = "\"time\":\"" + DateTime.UtcNow + "\", ";
                                message += "\"SystemProfile\":\"" + systemProfile + "\", ";
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    message += "\"" + reader.Name.Replace(" ", "_") + "\":\"" + reader.Value.Replace(" ", "_") + "\", ";
                                }
                                yield return message;
                            }
                            ///////////////Reading Measures

                            //First figure out what chart the measures are coming from
                              
                            if (reader.Name == "chartdashlet" && reader.HasAttributes)
                            {
                                
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    if (reader.Name == "name") currentChart = "\"" + reader.Value.Replace(" ", "_") + "\"";
                                }
                            }
                            //Then find out what splitting the measures are coming from
                            
                            if (reader.Name == "measure" && reader.HasAttributes)
                            {

                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    if (reader.Name == "measure") currentMeasure = "\"" + reader.Value.Replace(" ", "_") + "\"";
                                }
                            }

                            
                            if (reader.Name == "measurement" && reader.HasAttributes)
                            {
                                string message = "";
                                message = "\"time\":\"" + DateTime.UtcNow + "\", ";
                                message += "\"SystemProfile\":\"" + systemProfile + "\", ";
                                message += "\"Chart\":" + currentChart + ", ";
                                message += "\"Splitting\":" + currentMeasure + ", ";
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    message +=  "\"" + reader.Name.Replace(" ", "_") + "\":\"" + reader.Value.Replace(" ", "_") + "\", ";
                                }
                                yield return message;
                            }
                            break;
                     }
                 }       
             }
            yield break;
           

        }



    }
}


//Other option for querying a doc for elements at any depth
/*
       XDocument doc = XDocument.Parse(xml);

        foreach (XElement element in doc.Descendants("grandchild"))
        {
            Console.WriteLine(element);
        }

*/

//case XmlNodeType.Text:
//    writer.WriteString(reader.Value);
//    break;
////case XmlNodeType.XmlDeclaration:
////case XmlNodeType.ProcessingInstruction:
////    writer.WriteProcessingInstruction(reader.Name, reader.Value);
////    break;
////case XmlNodeType.Comment:
////    writer.WriteComment(reader.Value);
////    break;
////case XmlNodeType.EndElement:
////    writer.WriteFullEndElement();
////    break;
//case XmlNodeType.Attribute:
//    writer.WriteString(reader.Value);
//    break;


                //XmlWriterSettings ws = new XmlWriterSettings();
                //ws.Indent = true;
                //using (XmlWriter writer = XmlWriter.Create(output, ws))
                //{