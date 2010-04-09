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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer
{
	public sealed class ActionPlaceholderAttribute : ActionInitiatorAttribute
	{
		private readonly string _pathHint;
		private readonly string _groupHint = string.Empty;
		private bool _initiallyAvailable = true;

		public ActionPlaceholderAttribute(string actionId, string pathHint, string groupHint)
			: base(actionId)
		{
			_pathHint = pathHint;
			_groupHint = groupHint;
		}

		public bool InitiallyAvailable
		{
			get { return _initiallyAvailable; }
			set { _initiallyAvailable = value; }
		}

		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_pathHint, builder.ResourceResolver);
			builder.Action = new ActionPlaceholder(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Available = _initiallyAvailable;
			builder.Action.Persistent = true;
			builder.Action.Visible = false;
			builder.Action.Label = path.LastSegment.LocalizedText;
			builder.Action.GroupHint = new GroupHint(_groupHint ?? string.Empty);
		}
	}

	public class ActionPlaceholder : ClickAction
	{
		public ActionPlaceholder(string actionId, ActionPath path, IResourceResolver resourceResolver)
			: base(actionId, path, ClickActionFlags.None, resourceResolver) {}

		public static ActionPlaceholder GetPlaceholderAction(string site, IActionSet actions, string placeholderActionId)
		{
			string actionId = ":" + placeholderActionId;
			return (ActionPlaceholder) CollectionUtils.SelectFirst(actions, x => x is ActionPlaceholder && x.Path.StartsWith(new Path(site)) && x.ActionID.EndsWith(actionId));
		}

		public MenuAction CreateMenuAction(string actionId, string pathSuffix, ClickActionFlags flags)
		{
			return new DynamicMenuAction(actionId, pathSuffix, this, flags, this.ResourceResolver);
		}

		public MenuAction CreateMenuAction(string actionId, string pathSuffix, ClickActionFlags flags, IResourceResolver resourceResolver)
		{
			return new DynamicMenuAction(actionId, pathSuffix, this, flags, resourceResolver);
		}

		private class DynamicMenuAction : MenuAction
		{
			private readonly ActionPlaceholder _actionPlaceholder;
			private readonly Path _pathSuffix;

			public DynamicMenuAction(string actionId, string pathSuffix, ActionPlaceholder actionPlaceholder, ClickActionFlags flags, IResourceResolver resourceResolver)
				: base(actionId, actionPlaceholder.Path, flags, resourceResolver)
			{
				_actionPlaceholder = actionPlaceholder;
				_pathSuffix = new Path(pathSuffix);
			}

			private string BuildPath()
			{
				return _actionPlaceholder.Path.SubPath(0, _actionPlaceholder.Path.Segments.Count - 1).Append(_pathSuffix).ToString();
			}

			public override sealed ActionPath Path
			{
				get { return new ActionPath(this.BuildPath(), this.ResourceResolver); }
				set { }
			}

			public override sealed GroupHint GroupHint
			{
				get { return _actionPlaceholder.GroupHint; }
				set { }
			}

			public override sealed bool Available
			{
				get { return _actionPlaceholder.Available; }
				set { }
			}

			public override sealed bool Persistent
			{
				get { return false; }
			}
		}
	}
}