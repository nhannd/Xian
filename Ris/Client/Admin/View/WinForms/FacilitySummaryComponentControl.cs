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
    /// Provides a Windows Forms user-interface for <see cref="FacilitySummaryComponent"/>
    /// </summary>
    public partial class FacilitySummaryComponentControl : ApplicationComponentUserControl
    {
        private FacilitySummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacilitySummaryComponentControl(FacilitySummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _facilities.ToolbarModel = _component.FacilityListActionModel;
            _facilities.MenuModel = _component.FacilityListActionModel;

            _facilities.Table = _component.Facilities;
            _facilities.DataBindings.Add("Selection", _component, "SelectedFacility", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _facilities_Load(object sender, EventArgs e)
        {
            _component.LoadFacilityTable();
        }

        private void _facilities_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedFacility();
        }
    }
}
