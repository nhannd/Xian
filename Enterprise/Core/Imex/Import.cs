using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Import : ImexApplicationBase<ImportCommandLine>
    {
        protected override void Execute(ImportCommandLine cmdLine)
        {
            ImportOneClass(cmdLine);
        }

        private void ImportOneClass(ImportCommandLine cmdLine)
        {

            ImexUtils.ImportFromSingleFile(ImexUtils.FindImexForDataClass(cmdLine.DataClass), cmdLine.Path);
        }
    }
}
