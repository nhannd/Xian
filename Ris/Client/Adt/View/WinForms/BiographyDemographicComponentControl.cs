#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyDemographicComponent"/>
    /// </summary>
    public partial class BiographyDemographicComponentControl : ApplicationComponentUserControl
    {
        private BiographyDemographicComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyDemographicComponentControl(BiographyDemographicComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _selectedProfile.DataSource = _component.ProfileChoices;
            _selectedProfile.DataBindings.Add("Value", _component, "SelectedProfile", true, DataSourceUpdateMode.OnPropertyChanged);

            _addressList.Table = _component.Addresses;
            _addressList.DataBindings.Add("Selection", _component, "SelectedAddress", true, DataSourceUpdateMode.OnPropertyChanged);
            _phoneList.Table = _component.PhoneNumbers;
            _phoneList.DataBindings.Add("Selection", _component, "SelectedPhone", true, DataSourceUpdateMode.OnPropertyChanged);
            _emailList.Table = _component.EmailAddresses;
            _emailList.DataBindings.Add("Selection", _component, "SelectedEmail", true, DataSourceUpdateMode.OnPropertyChanged);
            _contactList.Table = _component.ContactPersons;
            _contactList.DataBindings.Add("Selection", _component, "SelectedContact", true, DataSourceUpdateMode.OnPropertyChanged);

            // Patient Identification fields
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrnSite.DataBindings.Add("Value", _component, "MrnSite", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardProvince.DataBindings.Add("Value", _component, "HealthcardProvince", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardVersionCode.DataBindings.Add("Value", _component, "HealthcardVersionCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardExpiry.DataBindings.Add("Value", _component, "HealthcardExpiry", true, DataSourceUpdateMode.OnPropertyChanged);

            // Personal Information fields
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);
            _prefix.DataBindings.Add("Value", _component, "Prefix", true, DataSourceUpdateMode.OnPropertyChanged);
            _suffix.DataBindings.Add("Value", _component, "Suffix", true, DataSourceUpdateMode.OnPropertyChanged);
            _degree.DataBindings.Add("Value", _component, "Degree", true, DataSourceUpdateMode.OnPropertyChanged);
            _sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfBirth.DataBindings.Add("Value", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);
            _timeOfDeath.DataBindings.Add("Value", _component, "TimeOfDeath", true, DataSourceUpdateMode.OnPropertyChanged);
            _religion.DataBindings.Add("Value", _component, "Religion", true, DataSourceUpdateMode.OnPropertyChanged);
            _primaryLanguage.DataBindings.Add("Value", _component, "PrimaryLanguage", true, DataSourceUpdateMode.OnPropertyChanged);        
        }

        private void _addressList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.ShowSelectedAddress();
        }

        private void _phoneList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.ShowSelectedPhone();
        }

        private void _emailList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.ShowSelectedEmail();
        }

        private void _contactList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.ShowSelectedContact();
        }
    }
}
