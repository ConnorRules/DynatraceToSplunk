using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splunk;
using System.IO;

namespace SplunkDataMigrator
{
    class Program
    {
        static string SplunkServer;
        static string SplunkAdmin;
        static string SplunkPass;
        static string DTUser;
        static string DTPass;
        static string DTServer;
        static string Dash;
        static string SplunkIndex;
        static string SplunkHost;

        static void Main(string[] args)
        {

            SplunkInterface Interfacer = new SplunkInterface();

            LoadConfig();
            
            //Send the homemade args to splunk
            if (Interfacer.Connect(SplunkServer, SplunkAdmin, SplunkPass)) Console.WriteLine("Connected Successfully");
            Interfacer.OpenStream(SplunkIndex, SplunkHost, "dynaTrace", "_json");

            //Get the dashboard from the REST interface
            RestInterface REST = new RestInterface();
            XmlInterface xmlint = new XmlInterface();

            int count = 0;
            foreach (string i in xmlint.Read(REST.Get(Dash, DTUser, DTPass, DTServer)))
            {
                Console.WriteLine("{" + i.Remove(i.Length - 2) + "}\n");
                Interfacer.WritetoStream("{" + i.Remove(i.Length - 2) + "}\n");     
                count++;
            }

            Console.WriteLine("Sent " + count + " events to Splunk Server.");

        }

        static void LoadConfig()
        {
            string line;
            try
            {

                System.IO.StreamReader file = new System.IO.StreamReader("config.txt");
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Split('\t')[0] == "SplunkServer") SplunkServer = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "SplunkAdmin") SplunkAdmin = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "SplunkPass") SplunkPass = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "SplunkIndex") SplunkIndex = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "SplunkHost") SplunkHost = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "DynaTraceServer") DTServer = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "DynaTraceUser") DTUser = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "DynaTracePass") DTPass = line.Split('\t')[1];
                    if (line.Split('\t')[0] == "Dashboard") Dash = line.Split('\t')[1];
                }

                file.Close();
                Console.WriteLine("Read file config.txt succesfully");
                
            }
            catch
            {
                Console.WriteLine("Could not read config.txt file");
                //Do stuff for manual entry
            }

        }
    }

}



//TODO
/*
add database reader
exceptions for connecting

*/