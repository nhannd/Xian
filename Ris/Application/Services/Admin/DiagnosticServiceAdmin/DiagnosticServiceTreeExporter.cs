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
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DiagnosticServiceTreeExporter : DataExporterBase
    {
        private const int _batchSize = 10;

        private const string tagTreeNodes = "diagnostic-service-tree";
        private const string tagTreeNode = "tree-node";
        private const string attrDescription = "description";
        private const string attrDiagnosticServiceCode = "diagnosticServiceCode";

        public override bool SupportsCsv
        {
            get { return false; }
        }

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            try
            {
                IDiagnosticServiceTreeNodeBroker broker = context.GetBroker<IDiagnosticServiceTreeNodeBroker>();

                // criteria for global root node
                DiagnosticServiceTreeNodeSearchCriteria where = new DiagnosticServiceTreeNodeSearchCriteria();
                where.Parent.IsNull();

                DiagnosticServiceTreeNode rootNode = broker.FindOne(where);

                writer.WriteStartDocument();
                writer.WriteStartElement(tagTreeNodes);

                foreach (DiagnosticServiceTreeNode node in rootNode.Children)
                {
                    WriteNodeXml(node, writer);
                }

                writer.WriteEndElement();

            }
            catch (EntityNotFoundException)
            {
                // no root node, nothing to export
            }
        }

        private void WriteNodeXml(DiagnosticServiceTreeNode node, XmlWriter writer)
        {
            writer.WriteStartElement(tagTreeNode);
            writer.WriteAttributeString(attrDescription, node.Description);
            if (node.DiagnosticService != null)
            {
                writer.WriteAttributeString(attrDiagnosticServiceCode, node.DiagnosticService.Id);
            }

            foreach (DiagnosticServiceTreeNode child in node.Children)
            {
                WriteNodeXml(child, writer);
            }

            writer.WriteEndElement();
        }

        //JR: This algorithm is nice but it doesn't export with the correct display order
        public override int ExportCsv(int batch, List<string> data, IReadContext context)
        {
            IDiagnosticServiceTreeNodeBroker broker = context.GetBroker<IDiagnosticServiceTreeNodeBroker>();

            // criteria for leaf nodes
            DiagnosticServiceTreeNodeSearchCriteria where = new DiagnosticServiceTreeNodeSearchCriteria();
            where.DiagnosticService.IsNotNull();

            IList<DiagnosticServiceTreeNode> leafNodes = broker.Find(where, new SearchResultPage(batch * _batchSize, _batchSize));

            foreach (DiagnosticServiceTreeNode leaf in leafNodes)
            {
                List<string> fields = new List<string>();
                fields.Add(leaf.DiagnosticService.Id);
                foreach (DiagnosticServiceTreeNode node in GetLineage(leaf))
                {
                    fields.Add(node.Description);
                }

                data.Add(MakeCsv(fields));
            }

            int totalLeafNodes = (int) broker.Count(where);
            int totalBatches = (int)Math.Ceiling((double)totalLeafNodes / (double)_batchSize);

            // remaining batches
            return totalBatches - (batch + 1);
        }

        private List<DiagnosticServiceTreeNode> GetLineage(DiagnosticServiceTreeNode leaf)
        {
            List<DiagnosticServiceTreeNode> lineage = new List<DiagnosticServiceTreeNode>();
            for (DiagnosticServiceTreeNode node = leaf; node != null; node = node.Parent)
            {
                lineage.Add(node);
            }
            lineage.Reverse();
            return lineage;
        }
    }
}
