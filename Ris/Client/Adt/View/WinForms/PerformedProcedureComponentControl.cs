using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformedProcedureComponent"/>
    /// </summary>
    public partial class PerformedProcedureComponentControl : ApplicationComponentUserControl
    {
        private PerformedProcedureComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponentControl(PerformedProcedureComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _mppsTableView.Table = _component.MppsTable;
            _mppsTableView.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);
            _mppsTableView.MenuModel = _component.MppsTableActionModel;
            _mppsTableView.ToolbarModel = _component.MppsTableActionModel;

            Control detailsPage = (Control)_component.DetailsComponentHost.ComponentView.GuiElement;
            detailsPage.Dock = DockStyle.Fill;
            _mppsDetailsPanel.Controls.Add(detailsPage);

        }

        private void _buttonCompleteDocumentationDetails_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.OnComplete();
            }
        }
    }
}
