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

using System;
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
    public class ProcedureTypeGroupExporter : DataExporterBase
    {
        private const string tagProcedureTypeGroups = "procedure-type-groups";

        private const string tagProcedureTypeGroup = "procedure-type-group";
        private const string attrName = "name";
        private const string attrClass = "class";
        private const string attrDescription = "description";

        private const string tagProcedureType = "procedure-type";

        #region DateExporter overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(tagProcedureTypeGroups);

            IList<ProcedureTypeGroup> groups = context.GetBroker<IProcedureTypeGroupBroker>().FindAll();
            CollectionUtils.ForEach<ProcedureTypeGroup>(groups,
                delegate(ProcedureTypeGroup group) { WriteProcedureTypeGroupXml(group, writer, context); });

            writer.WriteEndElement();
        }

        #endregion

        #region Private methods
        
        private void WriteProcedureTypeGroupXml(ProcedureTypeGroup group, XmlWriter writer, IReadContext context)
        {
            writer.WriteStartElement(tagProcedureTypeGroup);
            writer.WriteAttributeString(attrName, group.Name);
            writer.WriteAttributeString(attrClass, group.GetType().FullName);
            writer.WriteAttributeString(attrDescription, group.Description);

            CollectionUtils.ForEach<ProcedureType>(group.ProcedureTypes,
                delegate(ProcedureType type) { WriteProcedureTypeXml(type, writer); });

            writer.WriteEndElement();
        }

        private void WriteProcedureTypeXml(ProcedureType type, XmlWriter writer)
        {
            writer.WriteStartElement(tagProcedureType);
            writer.WriteAttributeString(attrName, type.Name);
            writer.WriteEndElement();
        }

        #endregion
    }
}
