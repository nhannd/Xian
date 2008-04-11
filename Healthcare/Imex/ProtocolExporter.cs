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
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
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
            CollectionUtils.ForEach<ProcedureTypeGroup>(group.ReadingGroups,
                delegate(ProcedureTypeGroup procedureTypeGroup) { WriteReadingGroupXml(procedureTypeGroup, writer); });
            writer.WriteEndElement();
        }

        private void WriteReadingGroupXml(ProcedureTypeGroup procedureTypeGroup, XmlWriter writer)
        {
            writer.WriteStartElement(tagReadingGroup);
            writer.WriteAttributeString(attrReadingGroupName, procedureTypeGroup.Name);
            writer.WriteEndElement();
        }

        #endregion
    }
}
