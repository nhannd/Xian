using System;
using System.Windows.Forms;

using ClearCanvas.Common;

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
                    ParseArg(arg, "OutputFile", ref outputFile);
                    ParseArg(arg, "DatabaseType", ref databaseType);
                }
            }
            
            DdlWriter writer = new DdlWriter();

            // generators will be processed in order they are added
            writer.AddGenerator(new DropSchemaGenerator(databaseType));
            writer.AddGenerator(new CreateSchemaGenerator(databaseType));
            writer.AddGenerator(new EnumValueInsertGenerator(databaseType));


            writer.Execute(outputFile);
        }

        private void ParseArg(string arg, string command, ref string val)
        {
            string lookFor = string.Format("/{0}:", command);
            if (arg.IndexOf(lookFor) > -1)
                val = arg.Replace(lookFor, "");
        }
    }
}
