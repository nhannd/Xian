#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ButtonAction("addGroup", "actionmodelconfig-toolbar/ToolbarAddGroup", "AddGroup")]
	[IconSet("addGroup", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[ButtonAction("addSeparator", "actionmodelconfig-toolbar/ToolbarAddSeparator", "AddSeparator")]
	[IconSet("addSeparator", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[ButtonAction("removeNode", "actionmodelconfig-toolbar/ToolbarRemoveNode", "RemoveNode")]
	[EnabledStateObserver("removeNode", "CanRemove", "CanRemoveChanged")]
	[IconSet("removeNode", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[ExtensionOf(typeof (ActionModelConfigurationComponentToolExtensionPoint))]
	public class BasicActionModelConfigurationComponentTools : ActionModelConfigurationComponentTool
	{
		public event EventHandler CanRemoveChanged;

		public bool CanRemove
		{
			get
			{
				if (base.SelectedNode == null)
					return false;
				if (base.SelectedNode is AbstractActionModelTreeLeafAction)
					return false;
				return true;
			}
		}

		protected override void OnSelectedNodeChanged()
		{
			base.OnSelectedNodeChanged();

			EventsHelper.Fire(this.CanRemoveChanged, this, EventArgs.Empty);
		}

		private void InsertNode(AbstractActionModelTreeNode node)
		{
			AbstractActionModelTreeNode selectedNode = base.SelectedNode;
			if (selectedNode is AbstractActionModelTreeBranch && !selectedNode.IsExpanded && selectedNode.Parent != null)
			{
				selectedNode.Parent.Children.Insert(selectedNode.Parent.Children.IndexOf(selectedNode) + 1, node);
			}
			else if (selectedNode is AbstractActionModelTreeBranch)
			{
				((AbstractActionModelTreeBranch) selectedNode).Children.Add(node);
				selectedNode.IsExpanded = true;
			}
			else if (selectedNode is AbstractActionModelTreeLeaf && selectedNode.Parent != null)
			{
				selectedNode.Parent.Children.Insert(selectedNode.Parent.Children.IndexOf(selectedNode) + 1, node);
			}
			else
			{
				base.Component.AbstractActionModelTreeRoot.Children.Add(node);
				base.Component.AbstractActionModelTreeRoot.IsExpanded = true;
			}
		}

		public void AddGroup()
		{
			this.InsertNode(new AbstractActionModelTreeBranch(SR.LabelNewGroup));
		}

		public void AddSeparator()
		{
			this.InsertNode(new AbstractActionModelTreeLeafSeparator());
		}

		public void RemoveNode()
		{
			AbstractActionModelTreeNode selectedNode = base.SelectedNode;
			if (this.CanRemove && selectedNode.Parent != null)
			{
				AbstractActionModelTreeBranch branch = selectedNode as AbstractActionModelTreeBranch;
				if (branch != null && !branch.IsEmpty)
				{
					base.Context.DesktopWindow.ShowMessageBox(SR.MessageNodeNotEmpty, MessageBoxActions.Ok);
					return;
				}

				selectedNode.Parent.Children.Remove(selectedNode);
			}
		}
	}
}