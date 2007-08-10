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

            int totalLeafNodes = broker.Count(where);
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
