#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class CustomizeViewerActionModelsComponentControl : CustomUserControl
	{
		private readonly CustomizeViewerActionModelsComponent _component;

		public CustomizeViewerActionModelsComponentControl(CustomizeViewerActionModelsComponent component)
		{
			InitializeComponent();

			AcceptButton = _btnOk;
			CancelButton = _btnCancel;

			_component = component;

			Control control = (Control) _component.TabComponentHost.ComponentView.GuiElement;
			control.Dock = DockStyle.Fill;
			_pnlMain.Controls.Add(control);
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
		    var currentCur = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                _component.Accept();
            }
            finally
            {
                Cursor.Current = currentCur;
            }

		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}