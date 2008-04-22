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

        public static void PrintImexDataClasses(TextWriter writer)
        {
            foreach (string w in ListImexDataClasses())
                writer.WriteLine(w);
        }


        public static void ExportToSingleFile(IXmDataImex imex, string path)
        {
            // delete the file if it exists (no error if it doesn't)
            File.Delete(path);

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
