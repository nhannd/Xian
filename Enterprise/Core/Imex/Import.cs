using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Import : ImexApplicationBase
    {
        protected override void Execute(ImexCommandLine cmdLine)
        {
            if (cmdLine.AllClasses)
            {
                // TODO
                throw new NotImplementedException();
            }
            else if (!string.IsNullOrEmpty(cmdLine.DataClass))
            {
                ImportOneClass(cmdLine);
            }
        }

        private void ImportOneClass(ImexCommandLine cmdLine)
        {

            ImexUtils.ImportFromSingleFile(FindImexForDataClass(cmdLine.DataClass), cmdLine.Path);
        }
    }
}
