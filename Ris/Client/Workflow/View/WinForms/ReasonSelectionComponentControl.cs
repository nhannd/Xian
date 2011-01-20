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
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProtocolReasonComponent"/>
	/// </summary>
	public partial class ReasonSelectionComponentControl : ApplicationComponentUserControl
	{
		private ReasonSelectionComponentBase _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReasonSelectionComponentControl(ReasonSelectionComponentBase component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_reason.DataSource = _component.ReasonChoices;
			_reason.DataBindings.Add("Value", _component, "SelectedReasonChoice", true, DataSourceUpdateMode.OnPropertyChanged);

			_otherReason.DataBindings.Add("Value", _component, "OtherReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_otherReason, _component.CannedTextLookupHandler);

			_btnOK.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _btnOK_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
