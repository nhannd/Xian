using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocolEditorComponent"/>
    /// </summary>
    public partial class ProtocolEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocolEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolEditorComponentControl(ProtocolEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            Control orderSummary = (Control)_component.OrderSummaryComponentHost.ComponentView.GuiElement;
            orderSummary.Dock = DockStyle.Fill;
            _orderSummaryPanel.Controls.Add(orderSummary);

            Control protocolNotesSummary = (Control)_component.ProtocolNotesSummaryComponentHost.ComponentView.GuiElement;
            protocolNotesSummary.Dock = DockStyle.Fill;
            _protocolNotesSummaryPanel.Controls.Add(protocolNotesSummary);

            _procedurePlanSummary.Table = _component.ProcedurePlanSummaryTable;
            _procedurePlanSummary.DataBindings.Add("Selection", _component, "SelectedRequestedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);
            //_procedurePlanSummary.MenuModel = _component.ProcedurePlanTreeActionModel;
            //_procedurePlanSummary.ToolbarModel = _component.ProcedurePlanTreeActionModel;
            _component.SelectionChanged += RefreshTables;

            protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
            protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;

            btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnReject.DataBindings.Add("Enabled", _component, "RejectEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSuspend.DataBindings.Add("Enabled", _component, "SuspendEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSave.DataBindings.Add("Enabled", _component, "SuspendEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void RefreshTables(object sender, EventArgs e)
        {
            protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
            protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;
        }

        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void btnReject_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reject();
            }
        }

        private void btnSuspend_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Suspend();
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Save();
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Close();
            }
        }

        private void btnAddNote_Click(object sender, System.EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.AddNote();
            }
        }
    }
}
