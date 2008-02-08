using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TechnologistDocumentationOrderDetailsComponent"/>
    /// </summary>
    public partial class TechnologistDocumentationOrderDetailsComponentControl : ApplicationComponentUserControl
    {
        private TechnologistDocumentationOrderDetailsComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistDocumentationOrderDetailsComponentControl(TechnologistDocumentationOrderDetailsComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            Control protocols = (Control)_component.ProtocolHost.ComponentView.GuiElement;
            protocols.Dock = DockStyle.Fill;
            _protocolsPanel.Controls.Add(protocols);

            Control notes = (Control)_component.NotesHost.ComponentView.GuiElement;
            notes.Dock = DockStyle.Fill;
            _orderNotesPanel.Controls.Add(notes);

            Control additionalInfo = (Control)_component.AdditionalInfoHost.ComponentView.GuiElement;
            additionalInfo.Dock = DockStyle.Fill;
            _additionalInfoPanel.Controls.Add(additionalInfo);
        }
    }
}
