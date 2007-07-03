using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();

			SetVersion();

			this._closeButton.ForeColor = Color.FromArgb(60, 150, 208);
			this._closeButton.Click += new EventHandler(OnCloseClicked);
		}

		private void SetVersion()
		{
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this._versionLabel.Text = String.Format(SR.FormatVersion, version);
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Close();
		}

	}
}