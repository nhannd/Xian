#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="WorklistPrintComponent"/>.
	/// </summary>
	public partial class WorklistPrintComponentControl : ApplicationComponentUserControl
	{
		private readonly WorklistPrintComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistPrintComponentControl(WorklistPrintComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			var browser = (Control)_component.WorklistPrintPreviewComponentHost.ComponentView.GuiElement;
			_previewPanel.Controls.Add(browser);
			browser.Dock = DockStyle.Fill;
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Print();
			}
		}

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
	}
}
