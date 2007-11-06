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
            //_procedurePlanSummary.MenuModel = _component.ProcedurePlanTreeActionModel;
            //_procedurePlanSummary.ToolbarModel = _component.ProcedurePlanTreeActionModel;
        }
    }
}
