using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.ProtocolAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProtocolExporter : DataExporterBase
    {
        private IReadContext _context;

        private const string tagProtocolGroups = "protocol-groups";

        private const string tagProtocolGroup = "protocol-group";
        private const string attrName = "name";
        private const string attrDescription = "description";

        private const string tagProtocolCodes = "protocol-codes";
        private const string tagProtocolCode = "protocol-code";
        private const string attrProtocolCodeName = "name";
        private const string attrProtocolCodeDescription = "description";

        private const string tagReadingGroups = "reading-groups";
        private const string tagReadingGroup = "reading-group";
        private const string attrReadingGroupName = "name";

        #region DataExporter overrides

        public override bool SupportsXml
        {
            get
            {
                return true;
            }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            _context = context;

            writer.WriteStartDocument();
            writer.WriteStartElement(tagProtocolGroups);

            IList<ProtocolGroup> protocolGroups = context.GetBroker<IProtocolGroupBroker>().FindAll();
            CollectionUtils.ForEach<ProtocolGroup>(protocolGroups,
                delegate(ProtocolGroup protocolGroup) { WriteProtocolGroupXml(protocolGroup, writer); });

            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        private void WriteProtocolGroupXml(ProtocolGroup protocolGroup, XmlWriter writer)
        {
            writer.WriteStartElement(tagProtocolGroup);
            writer.WriteAttributeString(attrName, protocolGroup.Name);
            writer.WriteAttributeString(attrDescription, protocolGroup.Description);

            WriteProtocolCodesXml(protocolGroup, writer);
            WriteReadingGroupsXml(protocolGroup, writer);

            writer.WriteEndElement();
        }

        private void WriteProtocolCodesXml(ProtocolGroup group, XmlWriter writer)
        {
            writer.WriteStartElement(tagProtocolCodes);
            CollectionUtils.ForEach<ProtocolCode>(group.Codes,
                delegate(ProtocolCode code) { WriteProtocolCodeXml(code, writer); });
            writer.WriteEndElement();
        }

        private void WriteProtocolCodeXml(ProtocolCode code, XmlWriter writer)
        {
            writer.WriteStartElement(tagProtocolCode);
            writer.WriteAttributeString(attrProtocolCodeName, code.Name);
            writer.WriteAttributeString(attrProtocolCodeDescription, code.Description);
            writer.WriteEndElement();
        }

        private void WriteReadingGroupsXml(ProtocolGroup group, XmlWriter writer)
        {
            writer.WriteStartElement(tagReadingGroups);
            CollectionUtils.ForEach<RequestedProcedureTypeGroup>(group.ReadingGroups,
                delegate(RequestedProcedureTypeGroup procedureTypeGroup) { WriteReadingGroupXml(procedureTypeGroup, writer); });
            writer.WriteEndElement();
        }

        private void WriteReadingGroupXml(RequestedProcedureTypeGroup procedureTypeGroup, XmlWriter writer)
        {
            writer.WriteStartElement(tagReadingGroup);
            writer.WriteAttributeString(attrReadingGroupName, procedureTypeGroup.Name);
            writer.WriteEndElement();
        }

        #endregion
    }
}
