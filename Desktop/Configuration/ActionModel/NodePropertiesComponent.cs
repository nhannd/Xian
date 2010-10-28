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
	[ExtensionPoint]
	public sealed class NodePropertiesComponentProviderExtensionPoint : ExtensionPoint<INodePropertiesComponentProvider> {}

	public interface INodePropertiesComponentProvider
	{
		IEnumerable<NodePropertiesComponent> CreateComponents(AbstractActionModelTreeNode selectedNode);
	}

	public abstract class NodePropertiesComponent : ApplicationComponent
	{
		private readonly AbstractActionModelTreeNode _selectedNode;

		protected NodePropertiesComponent(AbstractActionModelTreeNode selectedNode)
		{
			Platform.CheckForNullReference(selectedNode, "selectedNode");
			_selectedNode = selectedNode;
		}

		protected AbstractActionModelTreeNode SelectedNode
		{
			get { return _selectedNode; }
		}

		protected bool RequestPropertyValidation(string propertyName, object value)
		{
			return this.SelectedNode.RequestValidation(propertyName, value);
		}

		protected void NotifyPropertyValidated(string propertyName, object value)
		{
			this.SelectedNode.NotifyValidated(propertyName, value);
		}
	}
}