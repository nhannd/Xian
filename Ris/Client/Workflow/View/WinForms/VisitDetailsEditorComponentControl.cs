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
	/// Provides a Windows Forms user-interface for <see cref="VisitDetailsEditorComponent"/>
	/// </summary>
	public partial class VisitDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly VisitDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public VisitDetailsEditorComponentControl(VisitDetailsEditorComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			_visitNumber.DataBindings.Add("Value", _component, "VisitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_visitNumberAssigningAuthority.DataSource = _component.VisitNumberAssigningAuthorityChoices;
			_visitNumberAssigningAuthority.DataBindings.Add("Value", _component, "VisitNumberAssigningAuthority", true, DataSourceUpdateMode.OnPropertyChanged);

			_admitDateTime.DataBindings.Add("Value", _component, "AdmitDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_dischargeDateTime.DataBindings.Add("Value", _component, "DischargeDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_dischargeDisposition.DataBindings.Add("Value", _component, "DischargeDisposition", true, DataSourceUpdateMode.OnPropertyChanged);

			_vip.DataBindings.Add("Checked", _component, "Vip", true, DataSourceUpdateMode.OnPropertyChanged);
			_preadmitNumber.DataBindings.Add("Value", _component, "PreAdmitNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientClass.DataSource = _component.PatientClassChoices;
			_patientClass.DataBindings.Add("Value", _component, "PatientClass", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientType.DataSource = _component.PatientTypeChoices;
			_patientType.DataBindings.Add("Value", _component, "PatientType", true, DataSourceUpdateMode.OnPropertyChanged);

			_admissionType.DataSource = _component.AdmissionTypeChoices;
			_admissionType.DataBindings.Add("Value", _component, "AdmissionType", true, DataSourceUpdateMode.OnPropertyChanged);

			_visitStatus.DataSource = _component.VisitStatusChoices;
			_visitStatus.DataBindings.Add("Value", _component, "VisitStatus", true, DataSourceUpdateMode.OnPropertyChanged);

			_facility.DataSource = _component.FacilityChoices;
			_facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.Format += delegate(object sender, ListControlConvertEventArgs e)
								{
									e.Value = _component.FormatFacility(e.ListItem);
								};

			_currentLocation.DataSource = _component.CurrentLocationChoices;
			_currentLocation.DataBindings.Add("Value", _component, "CurrentLocation", true, DataSourceUpdateMode.OnPropertyChanged);
			_currentLocation.Format += delegate(object sender, ListControlConvertEventArgs e)
								{
									e.Value = _component.FormatCurrentLocation(e.ListItem);
								};

			_ambulatoryStatus.DataSource = _component.AmbulatoryStatusChoices;
			_ambulatoryStatus.DataBindings.Add("Value", _component, "AmbulatoryStatus", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
