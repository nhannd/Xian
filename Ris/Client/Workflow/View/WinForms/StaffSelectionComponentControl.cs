#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="StaffSelectionComponent"/>.
	/// </summary>
	public partial class StaffSelectionComponentControl : ApplicationComponentUserControl
	{
		private readonly StaffSelectionComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public StaffSelectionComponentControl(StaffSelectionComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_staff.LookupHandler = _component.StaffLookupHandler;
			_staff.DataBindings.Add("Value", _component, "Staff", true, DataSourceUpdateMode.OnPropertyChanged);
			_staff.DataBindings.Add("LabelText", _component, "LabelText", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, System.EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}
