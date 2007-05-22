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

            // Patient Identification fields
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrnSite.DataBindings.Add("Value", _component, "MrnSite", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "Healthcard", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardProvince.DataBindings.Add("Value", _component, "HealthcardProvince", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardVersionCode.DataBindings.Add("Value", _component, "HealthcardVersionCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcardExpiry.DataBindings.Add("Value", _component, "HealthcardExpiry", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
