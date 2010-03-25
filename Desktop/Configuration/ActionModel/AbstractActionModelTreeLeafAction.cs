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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public abstract class AbstractActionModelTreeLeaf : AbstractActionModelTreeNode
	{
		protected AbstractActionModelTreeLeaf(string resourceKey, string label)
			: base(resourceKey, label) {}

		protected AbstractActionModelTreeLeaf(PathSegment pathSegment)
			: base(pathSegment) {}

		public new bool Hidden
		{
			get { return base.Hidden; }
			set { base.Hidden = value; }
		}
	}

	public sealed class AbstractActionModelTreeLeafSeparator : AbstractActionModelTreeLeaf
	{
		public AbstractActionModelTreeLeafSeparator() : base(string.Empty, string.Empty) {}
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
		}

		internal IAction Action
		{
			get { return _action; }
		}

		public string ActionId
		{
			get { return _action.ActionId; }
		}

		public IconSet IconSet
		{
			get { return _action.IconSet; }
		}

		public IResourceResolver ResourceResolver
		{
			get { return _action.ResourceResolver; }
		}
	}
}