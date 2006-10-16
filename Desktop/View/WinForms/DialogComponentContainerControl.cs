using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class DialogComponentContainerControl : UserControl
	{
		private DialogComponentContainer _component;

		public DialogComponentContainerControl(DialogComponentContainer component)
		{
			_component = component;

			InitializeComponent();

			DialogContent content = _component.Content;
			Control contentControl = _component.ContentHost.ComponentView.GuiElement as Control;

			_contentPanel.Controls.Add(contentControl);
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
