using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines an extension point for XML data importer/exporters.
    /// </summary>
    [ExtensionPoint]
    public class XmlDataImexExtensionPoint : ExtensionPoint<IXmlDataImex>
    {
    }

    /// <summary>
    /// Abstract base class for <see cref="Import"/> and <see cref="Export"/> applications.
    /// </summary>
    public abstract class ImexApplicationBase<TCmdLine> : IApplicationRoot
        where TCmdLine : CommandLine, new()
    {
        #region IApplicationRoot Members

        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        public void RunApplication(string[] args)
        {
            TCmdLine cmdLine = new TCmdLine();

            try
            {
                cmdLine.Parse(args);
                Execute(cmdLine);
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Console.WriteLine("List of supported data-classes:");
                PrintImexDataClasses(Console.Out);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("Invalid data class: " + e.Message);
                Console.WriteLine("List of supported data-classes:");
                PrintImexDataClasses(Console.Out);
            }
        }

        #endregion

        /// <summary>
        /// Executes the action specified by the command line arguments.
        /// </summary>
        /// <param name="cmdLine"></param>
        protected abstract void Execute(TCmdLine cmdLine);


        private void PrintImexDataClasses(TextWriter writer)
        {
            foreach (string w in ImexUtils.ListImexDataClasses())
                writer.WriteLine(w);
        }

    }
}
