using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splunk;

namespace SplunkDataMigrator
{
    class SplunkInterface
    {
        private Service service = null;
        private bool connected = false;
        public System.IO.Stream iStream = null;


        public bool Connect(string hostname, string username, string pass)
        {
            // Create new ServiceArgs object to store 
            // connection info
            var connectArgs = new ServiceArgs
            {
                Host = hostname,
                Port = 8089
            };

            service = new Service(connectArgs);

            // Use the Login method to connect
            service.Login(username, pass);

            // Print received token to verify login
            System.Console.WriteLine("Token: ");
            System.Console.WriteLine(" " + service.Token);

            connected = true;

            //receiver = new Receiver(service);

            return connected;

        }

        public bool SendEvent()
        {
            Receiver splunkReceiver = new Receiver(service);


            var args = new Args();
            args.Add("host", "TodaysHostIs");
            args.Add("source", "dynaTrace");
            args.Add("sourcetype", "Monitoring");

            //splunkReceiver.Submit(args, "CurrentlyWorks");

            //The index needs to exist in the splunk database to work, otherwise a 400 error is thrown
            splunkReceiver.Submit("main", args, "EventType=4 Keywords=Classic, RecordNumber=number");

            Console.WriteLine("Submitted Successfully");

            return true;
        }

        public bool SendMessage(string message, string index = "main", string host = "localhost", 
            string source="dynaTrace", string sourcetype = "dynatrace/xml")
        {
            if (!connected) return false;

            Receiver splunkReceiver = new Receiver(service);
            
            var args = new Args();

            args.Add("host", host);
            args.Add("source", source);
            args.Add("sourcetype", sourcetype);

            splunkReceiver.Submit(index, args, message);

            return true;
        }

        public bool OpenStream(string index = "main", string host = "localhost",
    string source = "dynaTrace", string sourcetype = "dynatrace/xml")
        {
            if (!connected) return false;

            Receiver splunkReceiver = new Receiver(service);

            var args = new Args();

            args.Add("host", host);
            args.Add("source", source);
            args.Add("sourcetype", sourcetype);

            iStream = splunkReceiver.Attach("main", args);

            Console.WriteLine("Opened Input Stream");
            return true;
        }

        public bool CloseStream()
        {
            if (iStream == null) return false;

            try
            {
                iStream.Flush();
                iStream.Dispose();
                iStream.Close();
            }
            catch { }


            return true;
        }

        public bool WritetoStream(string message)
        {
            if (!connected) return false;
            if (iStream == null) return false;


            
            var bytes = Encoding.UTF8.GetBytes(message);
            iStream.Write(bytes, 0, bytes.Length);
            iStream.Flush();
 
            
            Console.WriteLine("Done");

            return true;
        }


    }
}
