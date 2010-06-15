#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Application for generating <see cref="ManifestInput"/> for use with <see cref="ManifestGenerationApplication"/>.
    /// </summary>
    /// <remarks>
    /// The ManifestInputGenerationApplication is used to generate a sample <see cref="ManifestInput"/> file
    /// that contains all the files contained in a given software distribution.  The output file can then 
    /// be edited and used with <see cref="ManifestGenerationApplication"/> to generate an actual 
    /// <see cref="ClearCanvasManifest"/> for a ClearCanvas product.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ManifestInputGenerationApplication : IApplicationRoot
    {
        #region Private Members

        private readonly ManifestInput _manifestInput = new ManifestInput();
        private readonly ManifestCommandLine _parms = new ManifestCommandLine();
        private bool _addedIgnoreLogs;
        
        #endregion Private Members

        #region Public Methods
        
        public void RunApplication(string[] args)
        {
            try
            {
                _parms.Parse(args);

                // Scan the specified directory
                ScanDirectory();

                // Save the manifest
                if (File.Exists(_parms.Manifest))
                    File.Delete(_parms.Manifest);

                Save();

                Environment.ExitCode = 0;
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                _parms.PrintUsage(Console.Out);
                Environment.ExitCode = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception when generating manifest input: {0}", e.Message);
                Environment.ExitCode = -1;
            }
        }

        #endregion Public Methods

        #region Private Methods
        
        private void Save()
        {
            using (FileStream fs = new FileStream(_parms.Manifest, FileMode.CreateNew))
            {
                XmlSerializer theSerializer = new XmlSerializer(typeof(ManifestInput));

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = Encoding.UTF8,
                };

                XmlWriter writer = XmlWriter.Create(fs, settings);
                if (writer != null)
                    theSerializer.Serialize(writer, _manifestInput);
                
                fs.Flush();
                fs.Close();
            }
        }

        private void ScanDirectory()
        {
            if (!Directory.Exists(_parms.DistributionDirectory))
            {
                throw new ApplicationException("Directory does not exist: " + _parms.DistributionDirectory);
            }

            FileProcessor.Process(_parms.DistributionDirectory, null,
                                     ScanFile, true);
        }

        private void ScanFile(string filePath, out bool cancel)
        {
            cancel = false;

            if (!filePath.StartsWith(_parms.DistributionDirectory)) 
                return;

            ManifestInput.InputFile input = new ManifestInput.InputFile
                                                {
                                                    Name = filePath.Substring(_parms.DistributionDirectory.Length)
                                                };

            if (input.Name.StartsWith("logs\\"))
            {
                if (!_addedIgnoreLogs)
                {
                    ManifestInput.InputFile inputLogs = new ManifestInput.InputFile
                                                            {
                                                                Ignore = true, 
                                                                Name = "logs/"
                                                            };
                    _manifestInput.Files.Add(inputLogs);
                    _addedIgnoreLogs = true;
                }
                return;
            }

            if (input.Name.EndsWith("exe") || input.Name.EndsWith("dll"))
                input.Signed = true;
            else
                input.Signed = false;

            if (input.Name.EndsWith("exe.config"))
                input.Config = true;

            _manifestInput.Files.Add(input);
        }

        #endregion Private Methods
    }
}
