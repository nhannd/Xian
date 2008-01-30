using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DiagnosticServiceExporter : DataExporterBase
    {
        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("diagnostic-services");

            IDiagnosticServiceBroker broker = context.GetBroker<IDiagnosticServiceBroker>();
            DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
            where.Id.SortAsc(0);

            IList<DiagnosticService> diagnosticServices = broker.Find(where);
            foreach (DiagnosticService ds in diagnosticServices)
            {
                WriteDiagnosticServiceXml(writer, ds);
            }

            writer.WriteEndElement();
        }

        private void WriteDiagnosticServiceXml(XmlWriter writer, DiagnosticService ds)
        {
            writer.WriteStartElement("diagnostic-service");
            writer.WriteAttributeString("id", ds.Id);
            writer.WriteAttributeString("name", ds.Name);

            writer.WriteStartElement("procedure-types");
            foreach (ProcedureType procedureType in ds.ProcedureTypes)
            {
                writer.WriteStartElement("procedure-type");
                writer.WriteAttributeString("id", procedureType.Id);
                writer.WriteAttributeString("name", procedureType.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
