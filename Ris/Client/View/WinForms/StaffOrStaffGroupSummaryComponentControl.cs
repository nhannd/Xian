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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="StaffOrStaffGroupSummaryComponent"/>.
	/// </summary>
	public partial class StaffOrStaffGroupSummaryComponentControl : ApplicationComponentUserControl
	{
		private readonly StaffOrStaffGroupSummaryComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public StaffOrStaffGroupSummaryComponentControl(StaffOrStaffGroupSummaryComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			var content = (Control)_component.TabComponentContainerHost.ComponentView.GuiElement;
			content.Dock = DockStyle.Fill;
			_tabHostPanel.Controls.Add(content);

			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
