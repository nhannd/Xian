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
	/// Provides a Windows Forms user-interface for <see cref="CancelOrderComponent"/>
	/// </summary>
	public partial class CancelOrderComponentControl : ApplicationComponentUserControl
	{
		private readonly CancelOrderComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public CancelOrderComponentControl(CancelOrderComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_proceduresTableView.Table = _component.ProceduresTable;
			_cancelReason.DataSource = _component.CancelReasonChoices;
			_cancelReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
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
