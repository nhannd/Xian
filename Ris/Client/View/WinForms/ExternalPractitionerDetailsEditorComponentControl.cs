#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerDetailsEditorComponent"/>
	/// </summary>
	public partial class ExternalPractitionerDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerDetailsEditorComponentControl(ExternalPractitionerDetailsEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			if (_component.HasWarning)
			{
				_warning.Text = _component.WarningMessage;
				_warning.Visible = true;
			}

			_familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
			_givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
			_middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);

			_licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_billingNumber.DataBindings.Add("Value", _component, "BillingNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_isVerified.DataBindings.Add("Checked", _component, "MarkVerified", true, DataSourceUpdateMode.OnPropertyChanged);
			_lastVerified.Text = _component.LastVerified;
			_isVerified.Visible= _component.CanVerify;
			_lastVerified.Visible = _component.CanVerify;
		}
	}
}
