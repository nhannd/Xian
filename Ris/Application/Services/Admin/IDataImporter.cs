using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.Xml;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public interface IDataImporter
    {
        bool SupportsCsv { get; }
        bool SupportsXml { get; }

        void ImportCsv(List<string> rows, IUpdateContext context);
        void ImportXml(XmlDocument xmlDocument, IUpdateContext context);
    }

    [ExtensionPoint]
    public class DataImporterExtensionPoint : ExtensionPoint<IDataImporter>
    {
    }

}
