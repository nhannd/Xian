using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using System.IO;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public abstract class DataImporterBase : IDataImporter, IApplicationRoot
    {
        private const int DEFAULT_BATCH_SIZE = 20;

        public DataImporterBase()
        {

        }


        #region IDataImporter Members

        public abstract void Import(List<string> rows, IUpdateContext context);

        #endregion

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length == 0)
            {
                Platform.Log(LogLevel.Error, "Name of data file to import must be supplied as first argument.");
                return;
            }

            int batch = 0;
            try
            {
                using (StreamReader reader = File.OpenText(args[0]))
                {
                    string line = null;
                    List<string> lines = new List<string>();

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);

                        if (lines.Count == DEFAULT_BATCH_SIZE)
                        {
                            // send batch
                            batch++;
                            ImportBatch(lines);
                            lines.Clear();
                        }
                    }

                    if (lines.Count > 0)
                    {
                        // send final batch
                        batch++;
                        ImportBatch(lines);
                    }
                }

            }
            catch (ImportException e)
            {
                // handle import exceptions so that we can add information about the 
                // row where the exception occured to the error message
                if (e.DataRow > -1)
                {
                    string message = string.Format("Error importing row {0}: {1}",
                        batch * DEFAULT_BATCH_SIZE + e.DataRow,
                        e.Message);

                    Platform.Log(LogLevel.Error, message);
                }
                else
                    Platform.Log(LogLevel.Error, e.Message);
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

        private void ImportBatch(List<string> rows)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                Import(rows, (IUpdateContext)PersistenceScope.Current);
                scope.Complete();
            }
        }

        #endregion
    }
}
