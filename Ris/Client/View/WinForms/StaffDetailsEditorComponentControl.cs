using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PractitionerDetailsEditorComponent"/>
    /// </summary>
    public partial class StaffDetailsEditorComponentControl : ApplicationComponentUserControl
    {
        private StaffDetailsEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponentControl(StaffDetailsEditorComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _licenseNumber.DataBindings.Add("Visible", _component, "IsPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
            _licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);

            _staffType.DataSource = _component.StaffTypeChoices;
            _staffType.DataBindings.Add("Value", _component, "StaffType", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
