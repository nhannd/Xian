using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public abstract class ImexApplicationBase<TCmdLine> : IApplicationRoot
        where TCmdLine : CommandLine, new()
    {
        #region IApplicationRoot Members

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

        private void PrintImexDataClasses(TextWriter writer)
        {
            foreach (string w in ImexUtils.ListImexDataClasses())
                writer.WriteLine(w);
        }


        protected abstract void Execute(TCmdLine cmdLine);
    }
}
