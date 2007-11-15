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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.Xml;
using System.IO;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public abstract class DataExporterBase : IDataExporter, IApplicationRoot
    {
        private const int DEFAULT_BATCH_SIZE = 20;

        public DataExporterBase()
        {
        }


        #region IDataExporter Members

        public virtual bool SupportsCsv
        {
            get { return false; }
        }

        public virtual bool SupportsXml
        {
            get { return false; }
        }

        public virtual int ExportCsv(int batch, List<string> data, IReadContext context)
        {
            throw new NotImplementedException();
        }

        public virtual void ExportXml(XmlWriter writer, IReadContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length == 0)
            {
                Log(LogLevel.Error, "Name of output file must be supplied as first argument.");
                return;
            }

            string path = args[0];

            if (!path.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && !path.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                Log(LogLevel.Error, "Unknown format - try appending .csv or .xml to the output file name.");
                return;
            }

            if (path.EndsWith(".xml") && !this.SupportsXml)
            {
                Log(LogLevel.Error, "This exporter does not support XML ouput format.");
                return;
            }
            else if (path.EndsWith(".csv") && !this.SupportsCsv)
            {
                Log(LogLevel.Error, "This exporter does not support CSV ouput format.");
                return;
            }


            try
            {
                using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
                {
                    if (path.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // treat as xml
                        using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
                        {
                            Log(LogLevel.Info, "Exporting...");

                            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
                            xmlWriter.Formatting = System.Xml.Formatting.Indented;

                            ExportXml(xmlWriter, (IReadContext)PersistenceScope.Current);
                            xmlWriter.Close();

                            scope.Complete();

                            Log(LogLevel.Info, "Completed.");
                        }
                    }
                    else
                    {
                        // treat as csv
                        for(int batch = 0, remaining = 1; remaining > 0; batch++)
                        {
                            List<string> data = new List<string>();
                            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
                            {
                                remaining = ExportCsv(batch++, data, (IReadContext)PersistenceScope.Current);
                                data.ForEach(delegate(string line) { writer.WriteLine(line); });
                                scope.Complete();
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log(LogLevel.Error, e.Message);
            }
        }

        #endregion

        #region Helpers

        protected string MakeCsv(IEnumerable<string> fields)
        {
            return StringUtilities.Combine<string>(fields, ",");
        }

        protected void Log(LogLevel level, string message)
        {
            Platform.Log(level, message);
        }

        #endregion
    }
}
