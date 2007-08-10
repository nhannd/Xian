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
            Console.WriteLine(message);
        }

        #endregion
    }
}
