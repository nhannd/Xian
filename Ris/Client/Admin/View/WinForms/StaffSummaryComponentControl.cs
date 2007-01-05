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
    /// Provides a Windows Forms user-interface for <see cref="StaffSummaryComponent"/>
    /// </summary>
    public partial class StaffSummaryComponentControl : ApplicationComponentUserControl
    {
        private StaffSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponentControl(StaffSummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _staffs.ToolbarModel = _component.StaffListActionModel;
            _staffs.MenuModel = _component.StaffListActionModel;

            _staffs.Table = _component.Staffs;
            _staffs.DataBindings.Add("Selection", _component, "SelectedStaff", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _staffs_Load(object sender, EventArgs e)
        {
            _component.LoadStaffTable();
        }

        private void _staffs_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedStaff();
        }
    }
}
