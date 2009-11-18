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

using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	internal interface IClickableHostedControl
	{
		event EventHandler ResetDropDownFocusRequested;
		event EventHandler CloseDropDownRequested;
	}

	internal sealed class ClickableToolStripControlHost : ToolStripControlHost
	{
		public ClickableToolStripControlHost(Control hostedControl) : base(hostedControl) {}
		public ClickableToolStripControlHost(Control hostedControl, string name) : base(hostedControl, name) {}

		protected override bool DismissWhenClicked
		{
			get { return false; }
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			base.OnSubscribeControlEvents(control);
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).ResetDropDownFocusRequested += Control_ResetDropDownFocusRequested;
				((IClickableHostedControl) control).CloseDropDownRequested += Control_CloseDropDownRequested;
			}
		}

		protected override void OnUnsubscribeControlEvents(Control control)
		{
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).CloseDropDownRequested -= Control_CloseDropDownRequested;
				((IClickableHostedControl) control).ResetDropDownFocusRequested -= Control_ResetDropDownFocusRequested;
			}
			base.OnUnsubscribeControlEvents(control);
		}

		private void Control_ResetDropDownFocusRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent != null)
				base.Parent.Focus();
		}

		private void Control_CloseDropDownRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent is ToolStripDropDown)
				((ToolStripDropDown) base.Parent).Close(ToolStripDropDownCloseReason.ItemClicked);
		}
	}
}