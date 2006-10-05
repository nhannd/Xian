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
    public partial class PatientAdminControl : UserControl
    {
        private PatientAdminComponent _component;

        public PatientAdminControl(PatientAdminComponent component)
        {
            InitializeComponent();

            _component = component;

            _patientTableView.Table = _component.WorkingSet;
            _component.WorkingSetChanged += new EventHandler(_component_WorkingSetChanged);
        }

        private void _component_WorkingSetChanged(object sender, EventArgs e)
        {
        }

        private void _patientTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.RowDoubleClick();
        }

        private void _patientTableView_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelection(_patientTableView.CurrentSelection);
        }
    }
}
