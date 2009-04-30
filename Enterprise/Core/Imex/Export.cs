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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Export application.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Export : ImexApplicationBase<ExportCommandLine>
    {
        /// <summary>
        /// Executes the action specified by the command line arguments.
        /// </summary>
        /// <param name="cmdLine"></param>
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

        /// <summary>
        /// Exports a single data-class.
        /// </summary>
        /// <param name="cmdLine"></param>
        private void ExportOneClass(ExportCommandLine cmdLine)
        {
            IXmlDataImex imex = ImexUtils.FindImexForDataClass(cmdLine.DataClass);
            int itemsPerFile = cmdLine.ItemsPerFile > 0
                                   ? cmdLine.ItemsPerFile
                                   : ImexUtils.GetDefaultItemsPerFile(imex.GetType());

            string directory;
            string baseFileName;

            // if the path has no extension, assume it specifies a directory
            if(string.IsNullOrEmpty(Path.GetExtension(cmdLine.Path)))
            {
                // use the name of the dataclass as a base filename
                directory = cmdLine.Path;
                baseFileName = cmdLine.DataClass;
            }
            else
            {
                // use the specified file name as the base filename
                directory = Path.GetDirectoryName(cmdLine.Path);
                baseFileName = Path.GetFileNameWithoutExtension(cmdLine.Path);
            }

            ImexUtils.Export(imex, directory, baseFileName, itemsPerFile);
        }

        /// <summary>
        /// Exports all data-classes for which an Imex extension exists.
        /// </summary>
        /// <param name="cmdLine"></param>
        private void ExportAllClasses(ExportCommandLine cmdLine)
        {
            string path = cmdLine.Path;

            // assume the supplied path is a directory, and create it if it doesn't exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (IXmlDataImex imex in new XmlDataImexExtensionPoint().CreateExtensions())
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
