using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StaffSummaryComponent"/>
    /// </summary>
    public partial class ExternalPractitionerSummaryComponentControl : ApplicationComponentUserControl
    {
        private ExternalPractitionerSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerSummaryComponentControl(ExternalPractitionerSummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _practitionerTableView.ToolbarModel = _component.PractitionersListActionModel;
            _practitionerTableView.MenuModel = _component.PractitionersListActionModel;

            _practitionerTableView.Table = _component.Practitioners;
            _practitionerTableView.DataBindings.Add("Selection", _component, "SelectedPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);

            _firstName.DataBindings.Add("Value", _component, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
            _lastName.DataBindings.Add("Value", _component, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);

            _okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
            _cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
        }

        private void _staffs_Load(object sender, EventArgs e)
        {
            //_component.LoadStaffTable();
        }

        private void _staffs_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedPractitioner();
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();        
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
