#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
