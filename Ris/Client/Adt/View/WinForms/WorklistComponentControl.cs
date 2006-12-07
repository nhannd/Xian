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
    /// Provides a Windows Forms user-interface for <see cref="PatientSearchResultComponent"/>
    /// </summary>
    public partial class WorklistComponentControl : CustomUserControl
    {
        private WorklistComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistComponentControl(WorklistComponent component)
        {
            InitializeComponent();

            _component = component;

            _patientProfileTable.Table = _component.SearchResults;
            _patientProfileTable.MenuModel = _component.MenuModel;
            _patientProfileTable.ToolbarModel = _component.ToolbarModel;

            _patientProfileTable.DataBindings.Add("Selection", _component, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _patientProfileTable_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickItem();
        }
    }
}
