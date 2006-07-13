using System;
using System.Windows.Forms;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DdlWriterApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length > 0)
            {
                string createFile = null;
                string dropFile = null;
                for (int i = 0; i < args.Length; i++)
                {
                    ParseArg(args[i], "create", ref createFile);
                    ParseArg(args[i], "drop", ref dropFile);
                }

                if (createFile == null && dropFile == null)
                {
                    PrintUsage();
                    return;
                }

                DdlWriter writer = new DdlWriter();
                writer.CreateSchemaFileName = createFile;
                writer.DropSchemaFileName = dropFile;
                writer.Execute();
            }
            else
            {
                // run GUI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DdlWriterForm());
            }
        }

        private void ParseArg(string arg, string command, ref string val)
        {
            string lookFor = string.Format("/{0}:", command);
            if (arg.IndexOf(lookFor) > -1)
                val = arg.Replace(lookFor, "");
        }

        private void PrintUsage()
        {
            Console.WriteLine("DDL Writer usage:");
            Console.WriteLine("/create:<file> /drop:<file>");
        }

        #endregion        
    }
}
