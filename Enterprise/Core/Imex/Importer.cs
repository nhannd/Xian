using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Importer : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            ExporterCommandLine cmdLine = new ExporterCommandLine();

            try
            {
                cmdLine.Parse(args);

                IXmDataImex imex = (IXmDataImex)new XmlDataImexExtensionPoint().CreateExtension(
                    delegate(ExtensionInfo info)
                    {
                        ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(info.ExtensionClass);
                        return a != null && a.DataClass == cmdLine.DataClass;
                    });

                ImexUtils.ImportFromSingleFile(imex, cmdLine.Path);
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Console.WriteLine("List of supported data-classes:");
                ImexUtils.PrintImexDataClasses(Console.Out);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("Invalid data class: " + e.Message);
                Console.WriteLine("List of supported data-classes:");
                ImexUtils.PrintImexDataClasses(Console.Out);
            }
        }

        #endregion
    }
}
