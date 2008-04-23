using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Import application.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Import : ImexApplicationBase<ImportCommandLine>
    {
        /// <summary>
        /// Executes the action specified by the command line arguments.
        /// </summary>
        /// <param name="cmdLine"></param>
        protected override void Execute(ImportCommandLine cmdLine)
        {
            IXmlDataImex imex = ImexUtils.FindImexForDataClass(cmdLine.DataClass);
            ImexUtils.Import(imex, cmdLine.Path);
        }
    }
}
