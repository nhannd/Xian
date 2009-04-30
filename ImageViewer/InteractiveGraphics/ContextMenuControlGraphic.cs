#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class ContextMenuControlGraphic : ControlGraphic, IContextMenuProvider
	{
		[CloneCopyReference]
		private IActionSet _actions;

		private string _namespace;
		private string _site;

		public ContextMenuControlGraphic(IGraphic subject)
			: this(string.Empty, string.Empty, null, subject) {}

		public ContextMenuControlGraphic(string site, IActionSet actions, IGraphic subject)
			: this(string.Empty, site, actions, subject) {}

		public ContextMenuControlGraphic(string @namespace, string site, IActionSet actions, IGraphic subject)
			: base(subject)
		{
			_namespace = @namespace;
			_site = site;
			_actions = actions;
		}

		protected ContextMenuControlGraphic(ContextMenuControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public virtual IActionSet Actions
		{
			get { return _actions; }
			set { _actions = value; }
		}

		public string Namespace
		{
			get
			{
				if (string.IsNullOrEmpty(_namespace))
					return typeof (ContextMenuControlGraphic).FullName;
				return _namespace;
			}
			protected set { _namespace = value; }
		}

		public string Site
		{
			get { return _site; }
			protected set { _site = value; }
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ActiveButton == XMouseButtons.Right)
			{
				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (this.HitTest(mouseInformation.Location))
					{
						return true;
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}
			return base.OnMouseStart(mouseInformation);
		}

		protected sealed override IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return this.Actions;
		}

		#region IContextMenuProvider Members

		public ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			if (string.IsNullOrEmpty(this.Site))
				return null;

			return ActionModelRoot.CreateModel(this.Namespace, this.Site, this.GetExportedActions(this.Site, mouseInformation));
		}

		#endregion
	}
}