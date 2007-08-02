using System;
using System.Windows.Forms;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DdlWriterApplication : IApplicationRoot
    {
        public void RunApplication(string[] args)
        {
            string outputFile = "model.ddl";
            string databaseType = "SQL";

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    TryParseArg(arg, "OutputFile", ref outputFile);
                    TryParseArg(arg, "DatabaseType", ref databaseType);
                }
            }

            Dialect dialect = (databaseType == "SQLite") ? (Dialect)new NHibernate.Dialect.SQLiteDialect() : (Dialect)new CustomSqlDialect();

            using (StreamWriter sw = File.CreateText(outputFile))
            {
                try
                {
                    PersistentStore store = new PersistentStore();
                    store.Initialize();

                    ScriptWriter scriptWriter = new ScriptWriter(store, dialect);

                    // for now, write the drop script first, and then write the create script to the same file
                    // in future, might be good to control this using command line flags
                    scriptWriter.WriteDropScript(sw);
                    scriptWriter.WriteCreateScript(sw);
                }
                catch (Exception e)
                {
                    Platform.Log(e, LogLevel.Error);
                    Console.WriteLine(e);
                }
            }
        }

        private bool TryParseArg(string arg, string command, ref string val)
        {
            string lookFor = string.Format("/{0}:", command);
            if (arg.IndexOf(lookFor) > -1)
            {
                val = arg.Replace(lookFor, "");
                return true;
            }
            return false;
        }
    }
}
