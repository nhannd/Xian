using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("DiagnosticServiceTree")]
    public class DiagnosticServiceTreeImex : XmlEntityImex<DiagnosticServiceTreeNode, DiagnosticServiceTreeImex.DiagnosticServiceTreeNodeData>
    {
        [DataContract]
        public class DiagnosticServiceTreeNodeData
        {
            [DataMember]
            public string Description;

            [DataMember]
            public string DiagnosticServiceId;

            [DataMember]
            public List<DiagnosticServiceTreeNodeData> Children;
        }

        #region Overrides
		
        protected override IList<DiagnosticServiceTreeNode> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            IDiagnosticServiceTreeNodeBroker broker = context.GetBroker<IDiagnosticServiceTreeNodeBroker>();

            // criteria for global root node
            DiagnosticServiceTreeNodeSearchCriteria where = new DiagnosticServiceTreeNodeSearchCriteria();
            where.Parent.IsNull();

            DiagnosticServiceTreeNode rootNode = CollectionUtils.FirstElement(broker.Find(where, new SearchResultPage(firstRow, maxRows)));
            return new DiagnosticServiceTreeNode[] { rootNode };
        }

        protected override DiagnosticServiceTreeNodeData Export(DiagnosticServiceTreeNode node, IReadContext context)
        {
            DiagnosticServiceTreeNodeData data = new DiagnosticServiceTreeNodeData();
            data.Description = node.Description;
            if (node.DiagnosticService != null)
            {
                data.DiagnosticServiceId = node.DiagnosticService.Id;
            }

            data.Children = CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeNodeData>(
                node.Children,
                delegate(DiagnosticServiceTreeNode child)
                {
                    return Export(child, context);
                });

            return data;
        }

        protected override void Import(DiagnosticServiceTreeNodeData data, IUpdateContext context)
        {
            Import(null, data, context);
        }

	    #endregion

        private void Import(DiagnosticServiceTreeNode parent, DiagnosticServiceTreeNodeData data, IUpdateContext context)
        {
            DiagnosticServiceTreeNode node = CreateNodeIfNotExists(data.Description, parent, context);

            if (!string.IsNullOrEmpty(data.DiagnosticServiceId))
            {
                node.DiagnosticService = GetDiagnosticService(data.DiagnosticServiceId, context);
            }

            if (data.Children != null)
            {
                foreach (DiagnosticServiceTreeNodeData childData in data.Children)
                {
                    Import(node, childData, context);
                }
            }
        }

        private DiagnosticServiceTreeNode CreateNodeIfNotExists(string description, DiagnosticServiceTreeNode parent,
            IPersistenceContext context)
        {
            DiagnosticServiceTreeNode node = null;
            if(parent != null)
            {
                node = CollectionUtils.SelectFirst(parent.Children,
                    delegate(DiagnosticServiceTreeNode n) { return n.Description == description; });
            }
            else
            {
                // look for an existing root node with this description
                DiagnosticServiceTreeNodeSearchCriteria where = new DiagnosticServiceTreeNodeSearchCriteria();
                where.Description.EqualTo(description);
                where.Parent.IsNull();

                node = CollectionUtils.FirstElement(context.GetBroker<IDiagnosticServiceTreeNodeBroker>().Find(where));
            }

            if (node == null)
            {
                node = new DiagnosticServiceTreeNode(parent);
                node.Description = description;

                context.Lock(node, DirtyState.New);
            }


            return node;
        }

        private DiagnosticService GetDiagnosticService(string code, IPersistenceContext context)
        {
            DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
            where.Id.EqualTo(code);
            return CollectionUtils.FirstElement(context.GetBroker<IDiagnosticServiceBroker>().Find(where));
        }
    }
}
