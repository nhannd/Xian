#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionOf(typeof (NodePropertiesComponentProviderExtensionPoint))]
	public sealed class ClickActionKeystrokePropertyProvider : INodePropertiesComponentProvider
	{
		public IEnumerable<NodePropertiesComponent> CreateComponents(AbstractActionModelTreeNode selectedNode)
		{
			if (selectedNode is AbstractActionModelTreeLeafClickAction)
				yield return new ClickActionKeystrokePropertyComponent((AbstractActionModelTreeLeafClickAction) selectedNode);
		}
	}

	[ExtensionPoint]
	public sealed class ClickActionKeystrokePropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ClickActionKeystrokePropertyComponentViewExtensionPoint))]
	public class ClickActionKeystrokePropertyComponent : NodePropertiesComponent
	{
		public ClickActionKeystrokePropertyComponent(AbstractActionModelTreeLeafClickAction selectedClickActionNode)
			: base(selectedClickActionNode) {}

		protected new AbstractActionModelTreeLeafClickAction SelectedNode
		{
			get { return (AbstractActionModelTreeLeafClickAction) base.SelectedNode; }
		}

		public XKeys KeyStroke
		{
			get { return this.SelectedNode.KeyStroke; }
			set
			{
				if (this.SelectedNode.KeyStroke != value)
				{
					this.SelectedNode.KeyStroke = value;
					this.NotifyPropertyChanged("KeyStroke");
				}
			}
		}

		public bool IsValidKeyStroke(XKeys keyStroke)
		{
			return this.SelectedNode.IsValidKeyStroke(keyStroke);
		}
	}
}