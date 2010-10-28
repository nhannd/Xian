#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
