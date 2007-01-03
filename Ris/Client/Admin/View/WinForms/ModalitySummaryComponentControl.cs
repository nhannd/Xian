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
    /// Provides a Windows Forms user-interface for <see cref="ModalitySummaryComponent"/>
    /// </summary>
    public partial class ModalitySummaryComponentControl : ApplicationComponentUserControl
    {
        private ModalitySummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalitySummaryComponentControl(ModalitySummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _modalities.ToolbarModel = _component.ModalityListActionModel;
            _modalities.MenuModel = _component.ModalityListActionModel;

            _modalities.Table = _component.Modalities;
            _modalities.DataBindings.Add("Selection", _component, "SelectedModality", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _modalities_Load(object sender, EventArgs e)
        {
            _component.LoadModalityTable();
        }

        private void _modalities_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedModality();
        }
    }
}
