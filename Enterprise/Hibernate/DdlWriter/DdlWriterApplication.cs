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

            if (args.Length > 0)
            {
                // maybe an explicit output file is specified 
            }

            DdlWriter writer = new DdlWriter();

            // generators will be processed in order they are added
            writer.AddGenerator(new DropSchemaGenerator());
            writer.AddGenerator(new CreateSchemaGenerator());
            writer.AddGenerator(new EnumValueInsertGenerator());


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
