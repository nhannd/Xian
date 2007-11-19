using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.ProtocolAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Protocol Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProtocolImporter : DataImporterBase
    {
        #region Private fields

        private IUpdateContext _context;

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

        #endregion

        #region DataImporterBase overrides

        public override bool SupportsXml
        {
            get
            {
                return true;
            }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;

            while (reader.Read())
            {
                if (reader.IsStartElement(tagProtocolGroups))
                {
                    ImportProtocolGroups(reader.ReadSubtree());
                }
            }
        }

        #endregion

        #region Private methods

        private void ImportProtocolGroups(XmlReader xmlReader)
        {
            for (xmlReader.ReadToDescendant(tagProtocolGroup);
                !(xmlReader.Name == tagProtocolGroups && xmlReader.NodeType == XmlNodeType.EndElement);
                xmlReader.ReadToNextSibling(tagProtocolGroup))
            {
                if (xmlReader.IsStartElement(tagProtocolGroup))
                {
                    ProcessProtocolGroupNode(xmlReader);
                }
            }
        }

        private void ProcessProtocolGroupNode(XmlReader reader)
        {
            string name = reader.GetAttribute(attrName);

            ProtocolGroup protocolGroup = LoadOrCreateProtocolGroup(name);
            if(protocolGroup != null)
            {
                string description = reader.GetAttribute(attrDescription);

                reader.ReadToFollowing(tagProtocolCodes);
                ICollection<ProtocolCode> protocolCodes = GetProtocolCodes(reader.ReadSubtree());

                reader.ReadToFollowing(tagReadingGroups);
                ICollection<RequestedProcedureTypeGroup> readingGroups = GetReadingGroups(reader.ReadSubtree());

                protocolGroup.Description = description;

                protocolGroup.Codes.Clear();
                CollectionUtils.ForEach<ProtocolCode>(protocolCodes,
                    delegate (ProtocolCode protocolCode) {protocolGroup.Codes.Add(protocolCode);});

                protocolGroup.ReadingGroups.Clear();
                protocolGroup.ReadingGroups.AddAll(readingGroups);
            }

        }

        private ProtocolGroup LoadOrCreateProtocolGroup(string name)
        {
            ProtocolGroupSearchCriteria criteria = new ProtocolGroupSearchCriteria();
            criteria.Name.EqualTo(name);

            IProtocolGroupBroker broker = _context.GetBroker<IProtocolGroupBroker>();
            ProtocolGroup protocolGroup = CollectionUtils.FirstElement<ProtocolGroup>(broker.Find(criteria));

            if(protocolGroup == null)
            {
                protocolGroup = new ProtocolGroup();
                protocolGroup.Name = name;

                _context.Lock(protocolGroup, DirtyState.New);
            }

            return protocolGroup;
        }

        private ICollection<RequestedProcedureTypeGroup> GetReadingGroups(XmlReader xmlReader)
        {
            List<RequestedProcedureTypeGroup> readingGroups = new List<RequestedProcedureTypeGroup>();

            for (xmlReader.ReadToDescendant(tagReadingGroups);
                !(xmlReader.Name == tagReadingGroups && xmlReader.NodeType == XmlNodeType.EndElement);
                xmlReader.Read())
            {
                if (xmlReader.IsStartElement(tagReadingGroup))
                {
                    RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
                    criteria.Name.EqualTo(xmlReader.GetAttribute(attrReadingGroupName));
                    criteria.Category.EqualTo(RequestedProcedureTypeGroupCategory.READING);

                    IRequestedProcedureTypeGroupBroker broker = _context.GetBroker<IRequestedProcedureTypeGroupBroker>();
                    RequestedProcedureTypeGroup group = CollectionUtils.FirstElement<RequestedProcedureTypeGroup>(broker.Find(criteria));
                    if (group != null) readingGroups.Add(group);
                }
            }

            return readingGroups;
        }

        private ICollection<ProtocolCode> GetProtocolCodes(XmlReader xmlReader)
        {
            List<ProtocolCode> protocolCodes = new List<ProtocolCode>();

            for (xmlReader.ReadToDescendant(tagProtocolCodes);
                !(xmlReader.Name == tagProtocolCodes && xmlReader.NodeType == XmlNodeType.EndElement);
                xmlReader.Read())
            {
                if (xmlReader.IsStartElement(tagProtocolCode))
                {
                    string codeName = xmlReader.GetAttribute(attrProtocolCodeName);
                    string codeDescription = xmlReader.GetAttribute(attrProtocolCodeDescription);

                    ProtocolCodeSearchCriteria criteria = new ProtocolCodeSearchCriteria();
                    criteria.Name.EqualTo(codeName);

                    IProtocolCodeBroker broker = _context.GetBroker<IProtocolCodeBroker>();
                    ProtocolCode code = CollectionUtils.FirstElement<ProtocolCode>(broker.Find(criteria));

                    if (code == null)
                    {
                        code = new ProtocolCode(codeName, codeDescription);
                        _context.Lock(code, DirtyState.New);
                    }
                        
                    protocolCodes.Add(code);
                }
            }

            return protocolCodes;
        }

        #endregion
    }
}
