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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ModalityEditorComponent"/>
	/// </summary>
	public partial class ModalityEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ModalityEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ModalityEditorComponentControl(ModalityEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_id.DataBindings.Add("Value", _component, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
			_aeTitle.DataBindings.Add("Value", _component, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);

			_facility.DataSource = _component.FacilityChoices;
			_facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatFacility(e.ListItem); };

			_dicomModality.DataSource = _component.DicomModalityChoices;
			_dicomModality.DataBindings.Add("Value", _component, "DicomModality", true, DataSourceUpdateMode.OnPropertyChanged);
			_dicomModality.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatDicomModality(e.ListItem); };

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
