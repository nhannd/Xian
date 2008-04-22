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
    public class Export : ImexApplicationBase
    {
        protected override void Execute(ImexCommandLine cmdLine)
        {
            if (cmdLine.AllClasses)
            {
                ExportAllClasses(cmdLine);
            }
            else if (!string.IsNullOrEmpty(cmdLine.DataClass))
            {
                ExportOneClass(cmdLine);
            }
        }

        private void ExportOneClass(ImexCommandLine cmdLine)
        {

            ImexUtils.ExportToSingleFile(FindImexForDataClass(cmdLine.DataClass), cmdLine.Path);
        }

        private void ExportAllClasses(ImexCommandLine cmdLine)
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
                    ImexUtils.ExportToSingleFile(imex, Path.Combine(path, a.DataClass + ".xml"));
                }
            }
        }
    }
}
