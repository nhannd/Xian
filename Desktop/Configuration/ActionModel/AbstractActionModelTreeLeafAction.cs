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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public abstract class AbstractActionModelTreeLeaf : AbstractActionModelTreeNode
	{
		protected AbstractActionModelTreeLeaf(PathSegment pathSegment)
			: base(pathSegment) {}
	}

	public sealed class AbstractActionModelTreeLeafSeparator : AbstractActionModelTreeLeaf
	{
		public AbstractActionModelTreeLeafSeparator() : base(new PathSegment("Separator", SR.LabelSeparator)) 
		{
			base.CheckState = CheckState.Checked;
		}

		internal Path GetSeparatorPath()
		{
			Stack<PathSegment> stack = new Stack<PathSegment>();
			stack.Push(new PathSegment(Guid.NewGuid().ToString()));

			AbstractActionModelTreeNode current = this.Parent;
			while (current != null)
			{
				stack.Push(current.PathSegment);
				current = current.Parent;
			}

			Path path = new Path(stack.Pop());
			while (stack.Count > 0)
			{
				path = path.Append(stack.Pop());
			}
			return path;
		}
	}

	public class AbstractActionModelTreeLeafAction : AbstractActionModelTreeLeaf
	{
		private readonly AbstractAction _action;

		public AbstractActionModelTreeLeafAction(IAction action)
			: base(action.Path.LastSegment)
		{
			Platform.CheckForNullReference(action, "action");
			Platform.CheckTrue(action.Persistent, "Action must be persistent.");

			// this allows us to keep a "clone" that is independent of the live action objects
			// that might (probably are) in use or cached in some tool or component somewhere.
			_action = AbstractAction.Create(action);

			base.CheckState = _action.Available ? CheckState.Checked : CheckState.Unchecked;
			base.IconSet = _action.IconSet;
			base.ResourceResolver = _action.ResourceResolver;
		}

		public string ActionId
		{
			get { return _action.ActionId; }
		}

		protected override void OnCheckStateChanged()
		{
			base.OnCheckStateChanged();

			_action.Available = this.CheckState == CheckState.Checked;
		}

		internal IAction GetAction()
		{
			IAction action = AbstractAction.Create(_action);

			Stack<PathSegment> stack = new Stack<PathSegment>();
			AbstractActionModelTreeNode current = this;
			do
			{
				stack.Push(current.PathSegment);
				current = current.Parent;
			} while (current != null);

			Path path = new Path(stack.Pop()); // the first path segment is the site, which is never processed through the resource resolver
			while (stack.Count > 0)
			{
				// for each subsequent segment, ensure the action's resolver will resolve the string in the expected way
				PathSegment pathSegment = stack.Pop();
				string localizedString = action.ResourceResolver.LocalizeString(pathSegment.ResourceKey);
				if (localizedString == pathSegment.LocalizedText)
					path = path.Append(pathSegment);
				else
					path = path.Append(new PathSegment(pathSegment.LocalizedText, pathSegment.LocalizedText));
			}

			action.Path = new ActionPath(path.ToString(), action.ResourceResolver);
			return action;
		}
	}
}