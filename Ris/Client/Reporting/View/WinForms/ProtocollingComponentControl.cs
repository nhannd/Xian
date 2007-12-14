using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocollingComponent"/>
    /// </summary>
    public partial class ProtocollingComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocollingComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocollingComponentControl(ProtocollingComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            Control orderSummary = (Control)_component.OrderSummaryComponentHost.ComponentView.GuiElement;
            orderSummary.Dock = DockStyle.Fill;
            _orderSummaryPanel.Controls.Add(orderSummary);

            Control protocolEditor = (Control)_component.ProtocolEditorComponentHost.ComponentView.GuiElement;
            protocolEditor.Dock = DockStyle.Fill;
            _protocolEditorPanel.Controls.Add(protocolEditor);

            Control orderDetails = (Control)_component.OrderDetailsHost.ComponentView.GuiElement;
            orderDetails.Dock = DockStyle.Fill;
            _orderDetailsPanel.Controls.Add(orderDetails);

        }
    }
}
