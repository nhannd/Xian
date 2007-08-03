using System;
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
            string outputFile = "";

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    TryParseArg(arg, "out", ref outputFile);
                }
            }

            Dialect dialect = new CustomSqlDialect();   // SQL Server 

            // if a file name was supplied, write to the file
            if (!string.IsNullOrEmpty(outputFile))
            {
                using (StreamWriter sw = File.CreateText(outputFile))
                {
                    WriteCreateScripts(sw, dialect);
                }
            }
            else
            {
                // by default write to stdout
                WriteCreateScripts(Console.Out, dialect);
            }
        }

        private void WriteCreateScripts(TextWriter writer, Dialect dialect)
        {
            try
            {
                PersistentStore store = new PersistentStore();
                store.Initialize();

                ScriptWriter scriptWriter = new ScriptWriter(store, dialect);
                scriptWriter.WriteCreateScript(writer);
            }
            catch (Exception e)
            {
                Log(e.Message, LogLevel.Error);
            }
        }

        private void Log(object obj, LogLevel level)
        {
            Platform.Log(LogLevel.Error, obj);
            Console.WriteLine(obj);
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
