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
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class SimpleComponentContainerControl : CustomUserControl
	{
		private SimpleComponentContainer _component;

		public SimpleComponentContainerControl(SimpleComponentContainer component)
		{
			_component = component;

			InitializeComponent();

			this.AcceptButton = _okButton;
			this.CancelButton = _cancelButton;

			Control contentControl = _component.ComponentHost.ComponentView.GuiElement as Control;

			// Make the dialog conform to the size of the content
			Size sizeDiff = contentControl.Size - _contentPanel.Size;

			_contentPanel.Controls.Add(contentControl);

			this.Size += sizeDiff;
			contentControl.Dock = DockStyle.Fill;

			_okButton.Click += new EventHandler(OnOkButtonClicked);
			_cancelButton.Click += new EventHandler(OnCancelButtonClicked);
		}

		void OnOkButtonClicked(object sender, EventArgs e)
		{
			_component.OK();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
