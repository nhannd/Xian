using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using System.IO;
using System.Xml;

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
            catch (Exception e)
            {
                Log(LogLevel.Error, e.Message);
            }
        }

        #endregion

        #region Helpers

        protected string[] ParseCsv(string line)
        {
            return line.Split(',');
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
            Console.WriteLine(message);
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
