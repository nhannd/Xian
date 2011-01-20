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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="CheckInOrderComponent"/>
	/// </summary>
	public partial class CheckInOrderComponentControl : ApplicationComponentUserControl
	{
		private readonly CheckInOrderComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public CheckInOrderComponentControl(CheckInOrderComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_orderTableView.Table = _component.OrderTable;

			_checkInDate.DataBindings.Add("Value", _component, "CheckInTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkInDate.DataBindings.Add("Visible", _component, "CheckInTimeVisible");
			_checkInTime.DataBindings.Add("Value", _component, "CheckInTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkInTime.DataBindings.Add("Visible", _component, "CheckInTimeVisible");

			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
