using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public static class ImexUtils
    {
        public static void ExportToSingleFile(IXmDataImex imex, string path)
        {
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);
                xmlWriter.Formatting = System.Xml.Formatting.Indented;
                xmlWriter.WriteStartElement("Items");
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
    }
}
