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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionPoint]
	public sealed class ActionModelConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ActionModelConfigurationComponentViewExtensionPoint))]
	public partial class ActionModelConfigurationComponent : ApplicationComponent, IConfigurationApplicationComponent
	{
		private event EventHandler _selectedNodeChanged;

		private readonly IDesktopWindow _desktopWindow;
		private readonly string _namespace;
		private readonly string _site;

		private ActionModelRoot _actionModel;
		private AbstractActionModelTreeRoot _actionModelTreeRoot;

		private ToolSet _toolSet;
		private ActionModelRoot _toolbarActionModel;

		private AbstractActionModelTreeNode _selectedNode;

		public ActionModelConfigurationComponent(string @namespace, string site, IActionSet actionSet, IDesktopWindow desktopWindow)
		{
			_namespace = @namespace;
			_site = site;
			_desktopWindow = desktopWindow;

			if (_desktopWindow is DesktopWindow)
			{
				DesktopWindow concreteDesktopWindow = (DesktopWindow) _desktopWindow;
				if (_site == DesktopWindow.GlobalMenus || _site == DesktopWindow.GlobalToolbars)
					actionSet = actionSet.Union(concreteDesktopWindow.DesktopTools.Actions);
			}

			_actionModel = ActionModelSettings.Default.BuildAbstractActionModel(_namespace, _site, actionSet.Select(a => a.Path.Site == site));
			_actionModelTreeRoot = new AbstractActionModelTreeRoot(_site);
			BuildActionModelTree(_actionModel, _actionModelTreeRoot);
		}

		public string ActionModelId
		{
			get { return string.Format("{0}:{1}", _namespace, _site); }
		}

		public ITree ActionModelTreeRoot
		{
			get { return _actionModelTreeRoot.Tree; }
		}

		public AbstractActionModelTreeRoot AbstractActionModelTreeRoot
		{
			get { return _actionModelTreeRoot; }
		}

		public AbstractActionModelTreeNode SelectedNode
		{
			get { return _selectedNode; }
			set
			{
				if (_selectedNode != value)
				{
					_selectedNode = value;
					this.OnSelectedNodeChanged();
				}
			}
		}

		public ActionModelRoot ToolbarActionModel
		{
			get { return _toolbarActionModel; }
		}

		protected virtual void OnSelectedNodeChanged()
		{
			EventsHelper.Fire(_selectedNodeChanged, this, EventArgs.Empty);
		}

		public event EventHandler SelectedNodeChanged
		{
			add { _selectedNodeChanged += value; }
			remove { _selectedNodeChanged -= value; }
		}

		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new ActionModelConfigurationComponentToolExtensionPoint(), new ActionModelConfigurationComponentToolContext(this));
			_toolbarActionModel = ActionModelRoot.CreateModel(this.GetType().FullName, "actionmodelconfig-toolbar", _toolSet.Actions);
		}

		public override void Stop()
		{
			_toolbarActionModel = null;
			_toolSet.Dispose();

			base.Stop();
		}

		public void Save()
		{
			ActionModelRoot actionModelRoot = _actionModelTreeRoot.GetAbstractActionModel();
			ActionModelSettings.Default.PersistAbstractActionModel(_namespace, _site, actionModelRoot);

			if (_desktopWindow is DesktopWindow)
			{
				DesktopWindow concreteDesktopWindow = (DesktopWindow) _desktopWindow;
				if (_site == DesktopWindow.GlobalMenus || _site == DesktopWindow.GlobalToolbars)
					concreteDesktopWindow.UpdateView();
			}
		}

		private static void BuildActionModelTree(ActionModelNode actionModel, AbstractActionModelTreeBranch abstractActionModelTreeBranch)
		{
			foreach (ActionModelNode childNode in actionModel.ChildNodes)
			{
				if (childNode is ActionNode)
				{
					ActionNode actionNode = (ActionNode) childNode;
					if (actionNode.Action.Persistent)
						abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafAction(actionNode.Action));
				}
				else if (childNode is SeparatorNode)
				{
					abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafSeparator());
				}
				else if (childNode is BranchNode)
				{
					AbstractActionModelTreeBranch treeBranch = new AbstractActionModelTreeBranch(childNode.PathSegment);
					BuildActionModelTree(childNode, treeBranch);
					abstractActionModelTreeBranch.AppendChild(treeBranch);
				}
			}
		}
	}
}