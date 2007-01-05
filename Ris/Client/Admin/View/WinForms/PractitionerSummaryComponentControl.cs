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
    /// Provides a Windows Forms user-interface for <see cref="PractitionerSummaryComponent"/>
    /// </summary>
    public partial class PractitionerSummaryComponentControl : ApplicationComponentUserControl
    {
        private PractitionerSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PractitionerSummaryComponentControl(PractitionerSummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _practitioners.ToolbarModel = _component.PractitionerListActionModel;
            _practitioners.MenuModel = _component.PractitionerListActionModel;

            _practitioners.Table = _component.Practitioners;
            _practitioners.DataBindings.Add("Selection", _component, "SelectedPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _practitioners_Load(object sender, EventArgs e)
        {
            _component.LoadPractitionerTable();
        }

        private void _practitioners_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedPractitioner();
        }
    }
}
