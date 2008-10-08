using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	internal partial class UpdateAvailableForm : Form
	{
		private string _downloadLink;

		private UpdateAvailableForm()
		{
			InitializeComponent();
		}

		public static void Show(string text, string downloadLink)
		{
			UpdateAvailableForm form = new UpdateAvailableForm();
			form._text.Text = text;
			form._downloadLink = downloadLink ?? "";

			form.ShowDialog();
		}

		private void OnOk(object sender, EventArgs e)
		{
			Close();
		}

		private void OnDownloadNow(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (String.IsNullOrEmpty(_downloadLink))
				return;

			try
			{
				Process.Start(_downloadLink);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, String.Format("Failed to navigate to download link '{0}'.", _downloadLink));
			}
		}
	}
}
