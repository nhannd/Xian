using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LocationSummaryComponent"/>
    /// </summary>
    public partial class LocationSummaryComponentControl : ApplicationComponentUserControl
    {
        private LocationSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationSummaryComponentControl(LocationSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _locations.ToolbarModel = _component.LocationListActionModel;
            _locations.MenuModel = _component.LocationListActionModel;

            _locations.Table = _component.Locations;
            _locations.DataBindings.Add("Selection", _component, "SelectedLocation", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _locations_Load(object sender, EventArgs e)
        {
            _component.LoadLocationTable();
        }

        private void _locations_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedLocation();
        }
    }
}
