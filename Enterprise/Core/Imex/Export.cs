using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionPoint]
    public class XmlDataImexExtensionPoint : ExtensionPoint<IXmDataImex>
    {
    }


    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Export : ImexApplicationBase<ExportCommandLine>
    {
        protected override void Execute(ExportCommandLine cmdLine)
        {
            if (cmdLine.AllClasses)
            {
                ExportAllClasses(cmdLine);
            }
            else if (!string.IsNullOrEmpty(cmdLine.DataClass))
            {
                ExportOneClass(cmdLine);
            }
            else 
                throw new CommandLineException("Must specify either /class:[data-class] or /all.");
        }

        private void ExportOneClass(ExportCommandLine cmdLine)
        {
            IXmDataImex imex = ImexUtils.FindImexForDataClass(cmdLine.DataClass);
            int itemsPerFile = cmdLine.ItemsPerFile > 0
                                   ? cmdLine.ItemsPerFile
                                   : ImexUtils.GetItemsPerFile(imex.GetType());
            if (cmdLine.ItemsPerFile == 0)
            {
                ImexUtils.ExportToSingleFile(imex, cmdLine.Path);
            }
            else
            {
                ImexUtils.Export(imex, cmdLine.Path, cmdLine.DataClass, itemsPerFile);
            }
        }

        private void ExportAllClasses(ExportCommandLine cmdLine)
        {
            string path = cmdLine.Path;

            // assume the supplied path is a directory, and create it if it doesn't exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (IXmDataImex imex in new XmlDataImexExtensionPoint().CreateExtensions())
            {
                ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(imex.GetType());
                if (a != null)
                {
                    ImexUtils.Export(imex, Path.Combine(path, a.DataClass), a.DataClass, a.ItemsPerFile);
                }
            }
        }
    }
}
