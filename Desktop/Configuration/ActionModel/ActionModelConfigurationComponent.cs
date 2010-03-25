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

using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionPoint]
	public class ActionModelConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ActionModelConfigurationComponentViewExtensionPoint))]
	public class ActionModelConfigurationComponent : ApplicationComponent
	{
		private readonly IDesktopWindow _desktopWindow;
		private readonly string _namespace;
		private readonly string _site;

		private readonly AbstractActionModelTreeRoot _unavailableActionsTreeRoot;
		private readonly AbstractActionModelTreeRoot _actionModelTreeRoot;

		public ActionModelConfigurationComponent(string @namespace, string site, IActionSet actionSet, IDesktopWindow desktopWindow)
		{
			if (desktopWindow is DesktopWindow)
			{
				DesktopWindow concreteDesktopWindow = (DesktopWindow) desktopWindow;
				if (site == DesktopWindow.GlobalMenus || site == DesktopWindow.GlobalToolbars)
					actionSet = actionSet.Union(concreteDesktopWindow.DesktopTools.Actions);
			}

			_namespace = @namespace;
			_site = site;
			_desktopWindow = desktopWindow;

			ActionModelRoot abstractActionModel = ActionModelSettings.Default.BuildAbstractActionModel(@namespace, site, actionSet);

			_unavailableActionsTreeRoot = new AbstractActionModelTreeRoot(_site);
			BuildUnavailableActionsTree(abstractActionModel, _unavailableActionsTreeRoot);

			_actionModelTreeRoot = new AbstractActionModelTreeRoot(_site);
			BuildActionModelTree(abstractActionModel, _actionModelTreeRoot);
		}

		public string ActionModelId
		{
			get { return string.Format("{0}:{1}", _namespace, _site); }
		}

		public ITree ActionModelTreeRoot
		{
			get { return _actionModelTreeRoot.Tree; }
		}

		public ITree UnavailableActionsTreeRoot
		{
			get { return _unavailableActionsTreeRoot.Tree; }
		}

		public void Save()
		{
			ActionModelRoot actionModelRoot = _actionModelTreeRoot.GetAbstractActionModel();
			foreach (IAction action in actionModelRoot.GetActionsInOrder())
				action.Available = true;

			ActionModelRoot unavailableActionsModelRoot = _unavailableActionsTreeRoot.GetAbstractActionModel();
			foreach (IAction action in unavailableActionsModelRoot.GetActionsInOrder())
				action.Available = false;

			actionModelRoot.Merge(unavailableActionsModelRoot);

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
					{
						if (actionNode.Action.Permissible)
						{
							if (actionNode.Action.Available)
								abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafAction(actionNode.Action), false);
						}
						else
						{
							abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafAction(actionNode.Action), true);
						}
					}
				}
				else if (childNode is SeparatorNode)
				{
					abstractActionModelTreeBranch.AppendChild(new AbstractActionModelTreeLeafSeparator(), false);
				}
				else if (childNode is BranchNode)
				{
					AbstractActionModelTreeBranch treeBranch = new AbstractActionModelTreeBranch(childNode.PathSegment);
					BuildActionModelTree(childNode, treeBranch);
					abstractActionModelTreeBranch.AppendChild(treeBranch, treeBranch.Children.Count == 0);
				}
			}
		}

		private static void BuildUnavailableActionsTree(ActionModelNode actionModel, AbstractActionModelTreeBranch abstractActionModelTreeBranch)
		{
			AbstractActionModelTreeBranch allActionsTreeBranch = new AbstractActionModelTreeBranch(SR.LabelAllActions);
			abstractActionModelTreeBranch.Children.Add(allActionsTreeBranch);

			List<AbstractActionModelTreeNode> list = new List<AbstractActionModelTreeNode>();
			foreach (IAction action in actionModel.GetActionsInOrder())
			{
				if (action.Persistent)
				{
					if (action.Permissible)
					{
						if (!action.Available)
							list.Add(new AbstractActionModelTreeLeafAction(action));
					}
				}
			}

			list.Sort((x, y) => x.CanonicalLabel.CompareTo(y.CanonicalLabel));
			foreach (AbstractActionModelTreeNode node in list)
			{
				allActionsTreeBranch.AppendChild(node, false);
			}
		}
	}
}