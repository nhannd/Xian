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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal sealed class ActionPlaceholderAttribute : ActionInitiatorAttribute
	{
		private readonly string _path;

		public ActionPlaceholderAttribute(string actionId, string pathHint)
			: base(actionId)
		{
			_path = pathHint;
		}

		public override void Apply(IActionBuildingContext builder)
		{
			// we use a custom action class instead of, say, ClickAction, because that prevents this placeholder action from
			// accidentally getting recognized as a publicly known type and allowing weird things like keystrokes to be
			// assigned to a placeholder.

			ActionPath path = new ActionPath(_path, builder.ResourceResolver);
			//builder.Action = new ActionPlaceholder(builder.ActionID, path, builder.ResourceResolver);
			builder.Action = new ClickAction(builder.ActionID, path, ClickActionFlags.None, builder.ResourceResolver);
			builder.Action.Persistent = true;
			builder.Action.Visible = false;
			builder.Action.Label = path.LastSegment.LocalizedText;
		}
	}

	//internal class ActionPlaceholder : Action
	//{
	//    public ActionPlaceholder(string actionId, ActionPath path, IResourceResolver resourceResolver) : base(actionId, path, resourceResolver) {}

	//    private class BeholdenAction : IAction {}
	//}
}