using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DowntimePrintFormsComponent"/>
    /// </summary>
    public partial class DowntimePrintFormsComponentControl : ApplicationComponentUserControl
    {
        private readonly DowntimePrintFormsComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DowntimePrintFormsComponentControl(DowntimePrintFormsComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_numberOfForms.DataBindings.Add("Value", _component, "NumberOfFormsToPrint", true, DataSourceUpdateMode.OnPropertyChanged);

			Control formPreview = (Control)_component.FormPreviewComponentHost.ComponentView.GuiElement;
			formPreview.Dock = DockStyle.Fill;
			this._formPreviewPanel.Controls.Add(formPreview);

			_component.AllPropertiesChanged += delegate
				{
					_statusText.Text = _component.StatusText;
					_progressBar.Maximum = _component.NumberOfFormsToPrint;
					_progressBar.Value = _component.NumberOfFormsPrinted;
					_numberOfForms.Enabled = !_component.IsPrinting;
					_printButton.Enabled = !_component.IsPrinting;
				};
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			_component.StartPrinting();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.CancelPrinting();
		}
    }
}
