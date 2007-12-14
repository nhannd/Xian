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

            Control orderNotes = (Control)_component.OrderNotesComponentHost.ComponentView.GuiElement;
            orderNotes.Dock = DockStyle.Fill;
            _orderNotesPanel.Controls.Add(orderNotes);

            _protocolGroup.DataSource = _component.ProtocolGroupChoices;
            _protocolGroup.DataBindings.Add("Value", _component, "ProtocolGroup", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.PropertyChanged += _component_PropertyChanged;

            _procedurePlanSummary.Table = _component.ProcedurePlanSummaryTable;
            _procedurePlanSummary.DataBindings.Add("Selection", _component, "SelectedRequestedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.SelectionChanged += RefreshTables;

            _protocolNextItem.DataBindings.Add("Checked", _component, "ProtocolNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
            _protocolNextItem.DataBindings.Add("Enabled", _component, "ProtocolNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
            protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;
            protocolCodesSelector.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnReject.DataBindings.Add("Enabled", _component, "RejectEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSuspend.DataBindings.Add("Enabled", _component, "SuspendEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSave.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSkip.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ProtocolGroupChoices")
            {
                _protocolGroup.DataSource = _component.ProtocolGroupChoices;
            }
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

        private void btnSkip_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Skip();
            }
        }
    }
}
