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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines utilities for working with Imex classes, and for transferring Imex data to and from the
    /// filesystem.
    /// </summary>
    public static class ImexUtils
    {
        #region ImportItem class

        private class ImportItem : IImportItem
        {
            private XmlReader _reader;

            public ImportItem(XmlReader reader)
            {
                _reader = reader;
            }

            public XmlReader Read()
            {
                return _reader;
            }
        }

        #endregion

        /// <summary>
        /// Defines the root XML tag under which data is exported.
        /// </summary>
        public const string RootTag = "Items";

        /// <summary>
        /// Returns a list of data-classes for which an Imex extension exists.
        /// </summary>
        /// <returns></returns>
        public static string[] ListImexDataClasses()
        {
            List<string> dataClasses = new List<string>();
            foreach (ExtensionInfo info in new XmlDataImexExtensionPoint().ListExtensions())
            {
                ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(info.ExtensionClass);
                if (a != null)
                    dataClasses.Add(a.DataClass);
            }
            return dataClasses.ToArray();
        }

        /// <summary>
        /// Finds the imex that supports the specified data-class.
        /// </summary>
        /// <param name="dataClass"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Indicates that no imex was found that supports the specified data-class.</exception>
        public static IXmlDataImex FindImexForDataClass(string dataClass)
        {
            return (IXmlDataImex)new XmlDataImexExtensionPoint().CreateExtension(
                delegate(ExtensionInfo info)
                {
                    return CollectionUtils.Contains(AttributeUtils.GetAttributes<ImexDataClassAttribute>(info.ExtensionClass),
                        delegate(ImexDataClassAttribute a)
                        {
                            return a != null && a.DataClass.Equals(
                                dataClass, StringComparison.InvariantCultureIgnoreCase);
                        });
                });
        }

        /// <summary>
        /// Gets the default number of items exported per file, for the specified Imex extension class.
        /// </summary>
        /// <param name="imexClass"></param>
        /// <returns></returns>
        public static int GetDefaultItemsPerFile(Type imexClass)
        {
            ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(imexClass);
            return a == null ? 0 : a.ItemsPerFile;
        }

        /// <summary>
        /// Exports data from the specified imex, to the specified directory using the specified base filename.
        /// </summary>
        /// <param name="imex"></param>
        /// <param name="directory"></param>
        /// <param name="baseFileName"></param>
        /// <param name="itemsPerFile"></param>
        public static void Export(IXmlDataImex imex, string directory, string baseFileName, int itemsPerFile)
        {
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // set a flag indicating whether all items should be exported to a single file
            bool oneFile = (itemsPerFile == 0);

            // if one file, set itemsPerFile to max value (which is effectively the same thing)
            if(oneFile)
                itemsPerFile = int.MaxValue;

            int itemCount = 0;
            int fileCount = 0;

            StreamWriter sw = null;
            XmlTextWriter writer = null;
            foreach (IExportItem item in imex.Export())
            {
                if(itemCount % itemsPerFile == 0)
                {
                    // close current file
                    if(writer != null)
                    {
                        writer.WriteEndElement();
                        writer.Close();
                        sw.Close();
                    }

                    // start new file
                    string file = oneFile ?  Path.Combine(directory, baseFileName)
                        : Path.Combine(directory, baseFileName + (++fileCount));
                    if (!file.EndsWith(".xml"))
                        file += ".xml";

                    // delete if already exists
                    File.Delete(file);

                    sw = new StreamWriter(File.OpenWrite(file));
                    writer = new XmlTextWriter(sw);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.WriteStartElement(RootTag);
                }

                item.Write(writer);

                itemCount++;
            }
            if (writer != null)
            {
                writer.WriteEndElement();
                writer.Close();
                sw.Close();
            }
        }

        /// <summary>
        /// Imports data to the specified imex, from the specified path, which may be either a single file
        /// or a directory containing a set of .xml files.
        /// </summary>
        /// <param name="imex"></param>
        /// <param name="path"></param>
        public static void Import(IXmlDataImex imex, string path)
        {
            Platform.CheckForNullReference(path, "path");   

            // determine list of source files to import
            List<string> fileList = new List<string>();
            if(File.Exists(path))
            {
                fileList.Add(path);
            }
            else if(Directory.Exists(path))
            {
                fileList.AddRange(Directory.GetFiles(path, "*.xml"));
            }
            else 
                throw new ArgumentException(string.Format("{0} is not a valid source file or directory.", path));

            imex.Import(ReadSourceFiles(fileList));
        }

        /// <summary>
        /// Reads the specified set of XML source files, yielding each item as an <see cref="IImportItem"/>.
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <returns></returns>
        private static IEnumerable<IImportItem> ReadSourceFiles(IEnumerable<string> sourceFiles)
        {
            foreach (string sourceFile in sourceFiles)
            {
                using (StreamReader reader = File.OpenText(sourceFile))
                {
                    XmlTextReader xmlReader = new XmlTextReader(reader);
                    xmlReader.WhitespaceHandling = WhitespaceHandling.None;

                    // advance to root tag
                    if (xmlReader.ReadToFollowing(RootTag))
                    {
                        // advance to first child
                        while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element) ;

                        // if child nodes exist, read them
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            string itemTag = xmlReader.Name;
                            for (bool more = true; more; more = xmlReader.ReadToNextSibling(itemTag))
                            {
                                yield return new ImportItem(xmlReader.ReadSubtree());
                            }
                        }
                    }
                    xmlReader.Close();
                }
            }
        }
    }
}
