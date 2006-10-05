using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using System.Xml;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class NUnitLogParser : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            XmlTextReader reader = new XmlTextReader(_filename);
            StreamWriter writer = new StreamWriter(_filename + ".log");
            string name;
            string success;
            string message;
            string stackTrace;

            writer.WriteLine("NUnit Failures for Build Performed on: " + DateTime.Now.ToString());
            writer.WriteLine();

            while (reader.Read())
            {
                if (reader.Name.EndsWith("test-case"))
                {

                    reader.MoveToNextAttribute();  //Name
                    name = reader.Value;
                    reader.MoveToNextAttribute(); //Executed
                    reader.MoveToNextAttribute(); //Success
                    success = reader.Value;

                    if (success == "False")
                    {
                        advanceToElement(ref reader, "message");
                        message = reader.ReadElementContentAsString();

                        advanceToElement(ref reader, "stack-trace");
                        stackTrace = reader.ReadElementContentAsString();

                        writer.Write(_failCount.ToString() + ") " + name);
                        _failCount++;
                        writer.WriteLine(" - " + message);
                        writer.WriteLine(stackTrace);
                        writer.WriteLine();
                    }
                }
            }

            writer.Close();
            reader.Close();
            return true;
        }

        private void advanceToElement(ref XmlTextReader reader, string element)
        {
            while (!reader.Name.EndsWith(element))
            {
                reader.Read();
            }
        }

        [Required]
        public string Filename
        {
            set { _filename = value; }
        }

        private string _filename;
        private int _failCount = 1;

    }
}
