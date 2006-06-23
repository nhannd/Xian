using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Workstation.Help
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();

			this.CloseButton.ForeColor = Color.FromArgb(60, 150, 208);
			this.CloseButton.Click += new EventHandler(CloseButton_Click);
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}