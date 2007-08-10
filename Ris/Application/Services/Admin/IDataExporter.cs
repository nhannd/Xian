using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public interface IDataExporter
    {
        bool SupportsCsv { get; }
        bool SupportsXml { get; }

        int ExportCsv(int batch, List<string> data, IReadContext context);
        void ExportXml(XmlWriter writer, IReadContext context);
    }

    [ExtensionPoint]
    public class DataExporterExtensionPoint : ExtensionPoint<IDataImporter>
    {
    }
}
