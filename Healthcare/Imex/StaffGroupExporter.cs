using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(DataExporterExtensionPoint), Name = "Staff Group Exporter")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StaffGroupExporter : DataExporterBase
    {
        private const string tagValue = "value";
        private const string tagStaffGroups = "staff-groups";
        private const string tagStaffGroup = "staff-group";
        private const string tagStaffMembers = "staff-members";

        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrId = "id";

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(tagStaffGroups);

            CollectionUtils.ForEach(context.GetBroker<IStaffGroupBroker>().FindAll(),
                delegate(StaffGroup group)
                {
                    WriteStaffGroup(group, writer);
                });

            writer.WriteEndElement();
        }

        private void WriteStaffGroup(StaffGroup group, XmlWriter writer)
        {
            writer.WriteStartElement(tagStaffGroup);
            writer.WriteAttributeString(attrName, group.Name);
            writer.WriteAttributeString(attrDescription, group.Description);

            writer.WriteStartElement(tagStaffMembers);
            foreach (Staff staff in group.Members)
            {
                writer.WriteStartElement(tagValue);
                writer.WriteAttributeString(attrId, staff.Id);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
