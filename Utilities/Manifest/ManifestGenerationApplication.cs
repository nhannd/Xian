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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Application for generating a <see cref="ClearCanvasManifest"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ManifestGenerationApplication : IApplicationRoot
    {
        private readonly ClearCanvasManifest _manifest = new ClearCanvasManifest();
        private readonly ManifestCommandLine _cmdLine = new ManifestCommandLine();
         
        #region Public Methods
        public void RunApplication(string[] args)
        {
             try
            {
                _cmdLine.Parse(args);

                List<ManifestInput> list = new List<ManifestInput>();
                foreach (string filename in _cmdLine.Positional)
                {
                    list.Add(Deserialize(filename));
                }

                if (_cmdLine.Package)
                {
                    _manifest.PackageManifest = new PackageManifest
                                                    {
                                                        Package = new Package()
                                                    };
                    _manifest.PackageManifest.Package.Manifest = _cmdLine.Manifest;
                    _manifest.PackageManifest.Package.Name = "Package";
                    _manifest.PackageManifest.Package.Product = new Product
                                                                    {
                                                                        Name = string.Empty
                                                                    };
                }
                else
                {
                    _manifest.ProductManifest = new ProductManifest
                                                    {
                                                        Product = new Product()
                                                    };
                    _manifest.ProductManifest.Product.Manifest = _cmdLine.Manifest;
                }

                ProcessConfiguration(list);

                ProcessFiles(list);

                Save();

                Environment.ExitCode = 0;             
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                _cmdLine.PrintUsage(Console.Out);
                Environment.ExitCode = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception when upgrading database: {0}", e.Message);
                Environment.ExitCode = -1;
            }
        }

        private void Save()
        {
            if (File.Exists(_cmdLine.Manifest))
                File.Delete(_cmdLine.Manifest);

            using (FileStream fs = new FileStream(_cmdLine.Manifest, FileMode.CreateNew))
            {
                XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = Encoding.UTF8,
                };

                XmlWriter writer = XmlWriter.Create(fs, settings);
                if (writer != null)
                    theSerializer.Serialize(writer, _manifest);
                fs.Flush();
                fs.Close();
            }
        }

        private void ProcessFiles(IEnumerable<ManifestInput> inputs)
        {
            foreach (ManifestInput input in inputs)
            {
                foreach(ManifestInput.InputFile inputFile in input.Files)
                {
                    string fullPath = Path.Combine(_cmdLine.DistributionDirectory, inputFile.Name);


                    if (!File.Exists(fullPath))
                    {
                        if (!Directory.Exists(fullPath))
                            continue;

                        ManifestFile directory = new ManifestFile
                                                     {
                                                         Ignore = inputFile.Ignore,
                                                         Filename = inputFile.Name
                                                     };
                        if (_manifest.ProductManifest != null)
                            _manifest.ProductManifest.Files.Add(directory);
                        else if (_manifest.PackageManifest != null)
                            _manifest.PackageManifest.Files.Add(directory);
                        continue;
                    }

                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fullPath);

                    FileInfo info = new FileInfo(fullPath);
                    ManifestFile file = new ManifestFile
                                            {
                                                Ignore = inputFile.Ignore,
                                                Filename = inputFile.Name,                                                
                                            };

                    if (inputFile.Signed)
                    {
                        file.Timestamp = info.CreationTimeUtc;
                        file.GenerateChecksum(fullPath);
                        file.Version = versionInfo.FileVersion;
                        file.Copyright = versionInfo.LegalCopyright;
                    }

                    if (_manifest.ProductManifest != null)
                        _manifest.ProductManifest.Files.Add(file);
                    else if (_manifest.PackageManifest !=null)
                        _manifest.PackageManifest.Files.Add(file);
                }
            }

        }

        private void ProcessConfiguration(IEnumerable<ManifestInput> inputs)
        {
            foreach (ManifestInput input in inputs)
            {
                foreach (ManifestInput.InputFile inputFile in input.Files)
                {
                    string fullPath = Path.Combine(_cmdLine.DistributionDirectory, inputFile.Name);

                    if (!File.Exists(fullPath)) continue;

                    if (inputFile.Config && _manifest.ProductManifest != null)
                    {
                        LoadConfiguration(fullPath);
                    }
                }
            }
        }

        private void LoadConfiguration(string path)
        {
            XmlDocument xmldoc = new XmlDocument();

            //Load Books.xml into the DOM.
            xmldoc.Load(path);

            XmlNodeList list = xmldoc.SelectNodes("configuration/applicationSettings/ClearCanvas.Common.ProductSettings/setting");

            if (list != null)
                foreach (XmlNode node in list)
                {
                    if (node.Attributes["name"] == null) continue;

                    string val = string.Empty;
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        val = childNode.InnerText;
                        val = ProductSettingsEncryption.Decrypt(val);
                        break;
                    }

                    if (node.Attributes["name"].Value.Equals("Name"))
                    {
                        _manifest.ProductManifest.Product.Name = val;
                    }
                    else if (node.Attributes["name"].Value.Equals("Version"))
                    {
                        _manifest.ProductManifest.Product.Version = val;
                    }
                    else if (node.Attributes["name"].Value.Equals("VersionSuffix"))
                    {
                        _manifest.ProductManifest.Product.Suffix = val;
                    }
                    else if (node.Attributes["name"].Value.Equals("Copyright"))
                    {

                    }
                    else if (node.Attributes["name"].Value.Equals("License"))
                    {

                    }
                }
        }


        private static ManifestInput Deserialize(string filename)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof(ManifestInput));


            using (FileStream fs = new FileStream(filename,FileMode.Open))
            {
                ManifestInput input = (ManifestInput)theSerializer.Deserialize(fs);

                return input;
            }
        }
        #endregion

        #region Private Static Methods
       

        #endregion
    }
}