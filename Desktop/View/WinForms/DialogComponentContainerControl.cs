using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class DialogComponentContainerControl : CustomUserControl
	{
		private DialogComponentContainer _component;

		public DialogComponentContainerControl(DialogComponentContainer component)
		{
			_component = component;

			InitializeComponent();

			this.AcceptButton = _okButton;
			this.CancelButton = _cancelButton;

			DialogContent content = _component.Content;
			Control contentControl = _component.ContentHost.ComponentView.GuiElement as Control;

			// Make the dialog conform to the size of the content
			Size sizeDiff = contentControl.Size - _contentPanel.Size;

			_contentPanel.Controls.Add(contentControl);

			this.Size += sizeDiff;
			contentControl.Dock = DockStyle.Fill;

			_okButton.Click += new EventHandler(OnOkButtonClicked);
			_cancelButton.Click += new EventHandler(OnCancelButtonClicked);
		}

		void OnOkButtonClicked(object sender, EventArgs e)
		{
			_component.OK();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
