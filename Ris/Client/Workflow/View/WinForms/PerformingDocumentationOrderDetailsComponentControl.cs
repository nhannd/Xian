using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PerformingDocumentationOrderDetailsComponent"/>
    /// </summary>
    public partial class PerformingDocumentationOrderDetailsComponentControl : ApplicationComponentUserControl
    {
        private PerformingDocumentationOrderDetailsComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformingDocumentationOrderDetailsComponentControl(PerformingDocumentationOrderDetailsComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            Control protocols = (Control)_component.ProtocolHost.ComponentView.GuiElement;
            protocols.Dock = DockStyle.Fill;
            _protocolsPanel.Controls.Add(protocols);

            Control notes = (Control)_component.NotesHost.ComponentView.GuiElement;
            notes.Dock = DockStyle.Fill;
            _orderNotesGroupBox.Controls.Add(notes);

            Control additionalInfo = (Control)_component.AdditionalInfoHost.ComponentView.GuiElement;
            additionalInfo.Dock = DockStyle.Fill;
            _additionalInfoPanel.Controls.Add(additionalInfo);
        }
    }
}
