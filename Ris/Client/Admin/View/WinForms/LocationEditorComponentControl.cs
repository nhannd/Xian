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
    /// Provides a Windows Forms user-interface for <see cref="LocationEditorComponent"/>
    /// </summary>
    public partial class LocationEditorComponentControl : ApplicationComponentUserControl
    {
        private LocationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationEditorComponentControl(LocationEditorComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _facility.DataSource = _component.FacilityChoices;
            _facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
            _building.DataBindings.Add("Value", _component, "Building", true, DataSourceUpdateMode.OnPropertyChanged);
            _floor.DataBindings.Add("Value", _component, "Floor", true, DataSourceUpdateMode.OnPropertyChanged);
            _pointOfCare.DataBindings.Add("Value", _component, "PointOfCare", true, DataSourceUpdateMode.OnPropertyChanged);
            _room.DataBindings.Add("Value", _component, "Room", true, DataSourceUpdateMode.OnPropertyChanged);
            _bed.DataBindings.Add("Value", _component, "Bed", true, DataSourceUpdateMode.OnPropertyChanged);
            _active.DataBindings.Add("Checked", _component, "Active", true, DataSourceUpdateMode.OnPropertyChanged);
            _inactiveDate.DataBindings.Add("Enabled", _component, "Active", true, DataSourceUpdateMode.OnPropertyChanged);
            _inactiveDate.DataBindings.Add("Value", _component, "InactiveDate", true, DataSourceUpdateMode.OnPropertyChanged);
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
