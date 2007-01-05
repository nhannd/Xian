using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PractitionerDetailsEditorComponent"/>
    /// </summary>
    public partial class PractitionerStaffDetailsEditorComponentControl : ApplicationComponentUserControl
    {
        private PractitionerStaffDetailsEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PractitionerStaffDetailsEditorComponentControl(PractitionerStaffDetailsEditorComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);
            _prefix.DataBindings.Add("Value", _component, "Prefix", true, DataSourceUpdateMode.OnPropertyChanged);
            _suffix.DataBindings.Add("Value", _component, "Suffix", true, DataSourceUpdateMode.OnPropertyChanged);
            _degree.DataBindings.Add("Value", _component, "Degree", true, DataSourceUpdateMode.OnPropertyChanged);
            if (_component.StaffMode)
            {
                _licenseNumber.Visible = false;
            }
            else
            {
                _licenseNumber.Visible = true;
                _licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
    }
}
