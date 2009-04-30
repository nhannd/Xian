#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
