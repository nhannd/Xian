#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public abstract class DataImporterBase : IDataImporter, IApplicationRoot
    {
        private const int DEFAULT_BATCH_SIZE = 20;

        public DataImporterBase()
        {
        }


        #region IDataImporter Members

        public virtual bool SupportsCsv
        {
            get { return false; }
        }

        public virtual bool SupportsXml
        {
            get { return false; }
        }

        public virtual void ImportCsv(List<string> rows, IUpdateContext context)
        {
            throw new NotImplementedException();
        }

        public virtual void ImportXml(XmlReader reader, IUpdateContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length == 0)
            {
                Platform.Log(LogLevel.Error, "Name of data file to import must be supplied as first argument.");
                return;
            }

            try
            {
                using (StreamReader reader = File.OpenText(args[0]))
                {
                    if(args[0].EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // treat as xml
                        XmlTextReader xmlReader = new XmlTextReader(reader);
                        xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                        using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                        {
                            ImportXml(xmlReader, (IUpdateContext)PersistenceScope.Current);
                            scope.Complete();
                        }
                        xmlReader.Close();
                    }
                    else
                    {
                        // treat as csv
                        List<string> lines = null;
                        while ((lines = ReadLines(reader, DEFAULT_BATCH_SIZE)).Count > 0)
                        {
                            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                            {
                                ImportCsv(lines, (IUpdateContext)PersistenceScope.Current);
                                scope.Complete();
                            }
                        }
                    }
                }

            }
            catch(EntityValidationException e)
            {
                Log(LogLevel.Error, e.MessageVerbose);
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, e.ToString());
            }
        }

        #endregion

        #region Helpers

        protected string[] ParseCsv(string line)
        {
            string[] fields = StringUtilities.SplitQuoted(line, ",");

            // replace empty strings with nulls
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Length == 0)
                    fields[i] = null;
            }
            return fields;
        }

        protected string[] ParseCsv(string line, int expectedFieldCount)
        {
            string[] fields = ParseCsv(line);
            if (fields.Length < expectedFieldCount)
                throw new ImportException(string.Format("Row must have {0} fields", expectedFieldCount));
            return fields;
        }

        protected void Log(LogLevel level, string message)
        {
            Platform.Log(level, message);
        }

        protected T TryParseOrDefault<T>(string value, T defaultValue)
        {
            T parsedValue;

            try
            {
                parsedValue = (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception)
            {
                parsedValue = defaultValue;
            }

            return parsedValue;
        }

        private List<string> ReadLines(StreamReader reader, int numLines)
        {
            List<string> lines = new List<string>();
            string line = null;
            while (lines.Count < numLines && (line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }
            return lines;
        }

        #endregion
    }
}
