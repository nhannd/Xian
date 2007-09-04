using System;
using System.Collections.Generic;
using System.Xml;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class RequestedProcedureTypeGroupExporter : DataExporterBase
    {
        private const string tagRequestedProcedureTypeGroups = "requested-procedure-type-groups";

        private const string tagRequestedProcedureTypeGroup = "requested-procedure-type-group";
        private const string attrName = "name";
        private const string attrCategory = "category";
        private const string attrDescription = "description";

        private const string tagRequestedProcedureType = "requested-procedure-type";

        #region DateExporter overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(tagRequestedProcedureTypeGroups);

            //List<Worklist> worklists = context.GetBroker<IWorklistBroker>().FindAll();
            //worklists.ForEach(delegate(Worklist worklist) { WriteWorklistXml(worklist, writer); });
            IList<RequestedProcedureTypeGroup> groups = context.GetBroker<IRequestedProcedureTypeGroupBroker>().FindAll();
            CollectionUtils.ForEach<RequestedProcedureTypeGroup>(groups,
                delegate(RequestedProcedureTypeGroup group) { WriteRequestedProcedureTypeGroupXml(group, writer, context); });

            writer.WriteEndElement();
        }

        #endregion

        #region Private methods
        
        private void WriteRequestedProcedureTypeGroupXml(RequestedProcedureTypeGroup group, XmlWriter writer, IReadContext context)
        {
            writer.WriteStartElement(tagRequestedProcedureTypeGroup);
            writer.WriteAttributeString(attrName, group.Name);
            writer.WriteAttributeString(attrCategory, EnumUtils.GetValue<RequestedProcedureTypeGroupCategory>(group.Category, context));
            writer.WriteAttributeString(attrDescription, group.Description);

            CollectionUtils.ForEach<RequestedProcedureType>(group.RequestedProcedureTypes,
                delegate(RequestedProcedureType type) { WriteRequestedProcedureTypeXml(type, writer); });

            writer.WriteEndElement();
        }

        private void WriteRequestedProcedureTypeXml(RequestedProcedureType type, XmlWriter writer)
        {
            writer.WriteStartElement(tagRequestedProcedureType);
            writer.WriteAttributeString(attrName, type.Name);
            writer.WriteEndElement();
        }

        #endregion
    }
}
