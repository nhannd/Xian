using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ClearCanvas.Desktop.Help
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();

			SetVersion();

			this._closeButton.ForeColor = Color.FromArgb(60, 150, 208);
			this._closeButton.Click += new EventHandler(CloseButton_Click);
		}

		private void SetVersion()
		{
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this._versionLabel.Text = String.Format(SR.FormatVersion, version);
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}