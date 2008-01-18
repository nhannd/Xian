#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
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
            for (bool elementExists = xmlReader.ReadToDescendant(tagProtocolGroup);
                elementExists;
                elementExists = xmlReader.ReadToNextSibling(tagProtocolGroup))
            {
                ProcessProtocolGroupNode(xmlReader.ReadSubtree());
            }
        }

        private void ProcessProtocolGroupNode(XmlReader reader)
        {
            reader.Read();

            string name = reader.GetAttribute(attrName);

            ProtocolGroup protocolGroup = LoadOrCreateProtocolGroup(name);
            if(protocolGroup != null)
            {
                string description = reader.GetAttribute(attrDescription);

                reader.ReadToDescendant(tagProtocolCodes);
                ICollection<ProtocolCode> protocolCodes = GetProtocolCodes(reader.ReadSubtree());

                reader.ReadToNextSibling(tagReadingGroups);
                ICollection<ProcedureTypeGroup> readingGroups = GetReadingGroups(reader.ReadSubtree());

                protocolGroup.Description = description;

                protocolGroup.Codes.Clear();
                CollectionUtils.ForEach<ProtocolCode>(protocolCodes,
                    delegate (ProtocolCode protocolCode) {protocolGroup.Codes.Add(protocolCode);});

                protocolGroup.ReadingGroups.Clear();
                protocolGroup.ReadingGroups.AddAll(readingGroups);
            }

            while (reader.Read()) ;
            reader.Close();
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

        private ICollection<ProcedureTypeGroup> GetReadingGroups(XmlReader xmlReader)
        {
            xmlReader.Read();

            List<ProcedureTypeGroup> readingGroups = new List<ProcedureTypeGroup>();

            for (bool elementExists = xmlReader.ReadToDescendant(tagReadingGroup);
                elementExists;
                elementExists = xmlReader.ReadToNextSibling(tagReadingGroup))
            {
                ReadingGroupSearchCriteria criteria = new ReadingGroupSearchCriteria();
                criteria.Name.EqualTo(xmlReader.GetAttribute(attrReadingGroupName));

                IReadingGroupBroker broker = _context.GetBroker<IReadingGroupBroker>();
                ReadingGroup group = CollectionUtils.FirstElement(broker.Find(criteria));
                if (group != null) readingGroups.Add(group);
            }

            while (xmlReader.Read()) ;
            xmlReader.Close();

            return readingGroups;
        }

        private ICollection<ProtocolCode> GetProtocolCodes(XmlReader xmlReader)
        {
            xmlReader.Read();

            List<ProtocolCode> protocolCodes = new List<ProtocolCode>();

            for (bool elementExists = xmlReader.ReadToDescendant(tagProtocolCode);
                elementExists;
                elementExists = xmlReader.ReadToNextSibling(tagProtocolCode))
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

            while (xmlReader.Read()) ;
            xmlReader.Close();

            return protocolCodes;
        }

        #endregion
    }
}
