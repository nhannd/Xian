using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public static class ImexUtils
    {
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


        public const string RootTag = "Items";

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
        public static IXmDataImex FindImexForDataClass(string dataClass)
        {
            return (IXmDataImex)new XmlDataImexExtensionPoint().CreateExtension(
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

        public static int GetItemsPerFile(Type imexClass)
        {
            ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(imexClass);
            return a == null ? 0 : a.ItemsPerFile;
        }

        public static void ExportToSingleFile(IXmDataImex imex, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (dir == path)
                throw new ArgumentException("Path must specify a file name.");

            // delete the file if it exists (no error if it doesn't)
            if (File.Exists(path))
                File.Delete(path);

            // create directories as needed
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);
                xmlWriter.Formatting = System.Xml.Formatting.Indented;
                xmlWriter.WriteStartElement(RootTag);
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    Platform.Log(LogLevel.Info, "Exporting...");

                    foreach (IExportItem item in imex.Export((IReadContext)PersistenceScope.Current))
                    {
                        item.Write(xmlWriter);
                    }

                    scope.Complete();

                    Platform.Log(LogLevel.Info, "Completed.");
                }
                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
        }

        public static void Export(IXmDataImex imex, string directory, string baseFileName, int itemsPerFile)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (itemsPerFile == 0)
                itemsPerFile = int.MaxValue;

            int itemCount = 0;
            int fileCount = 0;

            StreamWriter sw = null;
            XmlTextWriter writer = null;
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
            {
                foreach (IExportItem item in imex.Export((IReadContext)PersistenceScope.Current))
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
                        string file = Path.Combine(directory, baseFileName + (++fileCount) + ".xml");

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
                scope.Complete();
            }
            if (writer != null)
            {
                writer.WriteEndElement();
                writer.Close();
                sw.Close();
            }
        }

        public static void ImportFromSingleFile(IXmDataImex imex, string path)
        {
            try
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    XmlTextReader xmlReader = new XmlTextReader(reader);
                    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                    using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                    {
                        ((IUpdateContext)PersistenceScope.Current).ChangeSetRecorder.OperationName = imex.GetType().FullName;

                        // advance to root tag
                        if (xmlReader.ReadToFollowing(RootTag))
                        {
                            // advance to first child
                            while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element) ;

                            // if child nodes exist, read them
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                imex.Import(ReadItems(xmlReader, xmlReader.Name), (IUpdateContext)PersistenceScope.Current);
                            }

                        }

                        scope.Complete();
                    }
                    xmlReader.Close();
                }

            }
            catch(EntityValidationException e)
            {
                Platform.Log(LogLevel.Error, e.MessageVerbose);
            }
        }

        private static IEnumerable<IImportItem> ReadItems(XmlReader reader, string itemTag)
        {
            for (bool more = true; more; more = reader.ReadToNextSibling(itemTag))
            {
                yield return new ImportItem(reader.ReadSubtree());
            }
        }
    }
}
