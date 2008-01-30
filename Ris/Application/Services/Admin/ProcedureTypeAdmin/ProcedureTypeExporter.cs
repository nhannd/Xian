using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProcedureTypeExporter : DataExporterBase
    {
        public override bool SupportsXml
        {
            get
            {
                return true;
            }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("procedure-types");

            IProcedureTypeBroker broker = context.GetBroker<IProcedureTypeBroker>();
            ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
            where.Id.SortAsc(0);
            
            IList<ProcedureType> types = broker.Find(where);
            foreach (ProcedureType type in types)
            {
                WriteProcedureTypeXml(writer, type);
            }

            writer.WriteEndElement();
        }

        private void WriteProcedureTypeXml(XmlWriter writer, ProcedureType type)
        {
            writer.WriteStartElement("procedure-type");
            writer.WriteAttributeString("id", type.Id);
            writer.WriteAttributeString("name", type.Name);
            if(type.BaseType != null)
            {
                writer.WriteAttributeString("baseType", type.BaseType.Id);
            }

            XmlDocument plan = type.GetPlanXml();
            if (plan.DocumentElement != null)
            {
                XmlDocumentFragment fragment = plan.CreateDocumentFragment();
                fragment.AppendChild(plan.DocumentElement);
                fragment.WriteTo(writer);
            }

            writer.WriteEndElement();
        }
    }
}