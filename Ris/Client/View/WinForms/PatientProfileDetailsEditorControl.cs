#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class PatientProfileDetailsEditorControl : ApplicationComponentUserControl
    {
        private PatientProfileDetailsEditorComponent _component;

        public PatientProfileDetailsEditorControl(PatientProfileDetailsEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            // create bindings
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);

            _sex.DataSource = _component.SexChoices;
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);

            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfDeath.DataBindings.Add("Value", _component, "TimeOfDeath", true, DataSourceUpdateMode.OnPropertyChanged);

            _mrn.DataBindings.Add("Value", _component, "MrnID", true, DataSourceUpdateMode.OnPropertyChanged);
			_mrn.DataBindings.Add("Enabled", _component, "MrnIDEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _mrnAuthority.DataSource = _component.MrnAuthorityChoices;
            _mrnAuthority.DataBindings.Add("Value", _component, "MrnAuthority", true, DataSourceUpdateMode.OnPropertyChanged);
			_mrnAuthority.DataBindings.Add("Enabled", _component, "MrnAuthorityEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _healthcard.DataBindings.Add("Value", _component, "HealthcardID", true, DataSourceUpdateMode.OnPropertyChanged);

            _insurer.DataSource = _component.HealthcardAuthorityChoices;
            _insurer.DataBindings.Add("Value", _component, "HealthcardAuthority", true, DataSourceUpdateMode.OnPropertyChanged);

            _healthcardVersionCode.DataBindings.Add("Value", _component, "HealthcardVersionCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardExpiry.DataBindings.Add("Value", _component, "HealthcardExpiryDate", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void PatientEditorControl_Load(object sender, EventArgs e)
        {
            _dateOfBirth.Mask = _component.DateOfBirthMask;
            _healthcard.Mask = _component.HealthcardMask;
            _healthcardVersionCode.Mask = _component.HealthcardVersionCodeMask;
        }
    }
}