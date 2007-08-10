using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Diagnostic Service Tree Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DiagnosticServiceTreeImporter : DataImporterBase
    {
        private const string tagTreeNodes = "diagnostic-service-tree";
        private const string tagTreeNode = "tree-node";
        private const string attrDescription = "description";
        private const string attrDiagnosticServiceCode = "diagnosticServiceCode";

        private IUpdateContext _context;


        public DiagnosticServiceTreeImporter()
        {

        }
        
        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;

            IDiagnosticServiceTreeNodeBroker broker = _context.GetBroker<IDiagnosticServiceTreeNodeBroker>();

            DiagnosticServiceTreeNode rootNode = null;
            try
            {
                // criteria for root node
                DiagnosticServiceTreeNodeSearchCriteria where = new DiagnosticServiceTreeNodeSearchCriteria();
                where.Parent.IsNull();

                rootNode = broker.FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                rootNode = new DiagnosticServiceTreeNode(null);
                rootNode.Description = "ROOT";

                _context.Lock(rootNode, DirtyState.New);
            }

            while (reader.Read())
            {
                if (reader.IsStartElement(tagTreeNode))
                {
                    ReadTreeNode(reader.ReadSubtree(), rootNode);
                }
            }
        }

        private void ReadTreeNode(XmlReader reader, DiagnosticServiceTreeNode parent)
        {
            reader.Read();

            string description = reader.GetAttribute(attrDescription);
            string code = reader.GetAttribute(attrDiagnosticServiceCode);

            DiagnosticServiceTreeNode node = CreateNodeIfNotExists(description, code, parent);
            if(reader.ReadToDescendant(tagTreeNode))
            {
                ReadTreeNode(reader.ReadSubtree(), node);
                while (reader.ReadToNextSibling(tagTreeNode))
                {
                    ReadTreeNode(reader.ReadSubtree(), node);
                }
            }
        }

        private DiagnosticServiceTreeNode CreateNodeIfNotExists(string description, string dsCode, DiagnosticServiceTreeNode parent)
        {
            DiagnosticServiceTreeNode node = CollectionUtils.SelectFirst<DiagnosticServiceTreeNode>(parent.Children,
                delegate(DiagnosticServiceTreeNode n) { return n.Description == description; });

            if (node == null)
            {
                node = new DiagnosticServiceTreeNode(parent);
                node.Description = description;

                _context.Lock(node, DirtyState.New);

                if (!string.IsNullOrEmpty(dsCode))
                {
                    DiagnosticService ds = GetDiagnosticService(dsCode);
                    node.DiagnosticService = ds;
                }

            }
            return node;
        }

        private DiagnosticService GetDiagnosticService(string code)
        {
            try
            {
                IDiagnosticServiceBroker broker = _context.GetBroker<IDiagnosticServiceBroker>();
                DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
                where.Id.EqualTo(code);
                return broker.FindOne(where);

            }
            catch (EntityNotFoundException)
            {
                Log(LogLevel.Error, string.Format("Diagnostic Service Code {0} does not exist.", code));
                return null;
            }
        }
    }
}
