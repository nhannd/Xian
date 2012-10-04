#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

namespace ClickOncePublisher
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			UpdateButtons();
		}

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			ProductProfileForm form = new ProductProfileForm();
			form.MdiParent = this;
			form.WindowState = FormWindowState.Maximized;
			form.Show();
		}

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			string filename = CommonDialogHelper.GetOpenFilename();

			if (filename == null)
				return;

			ProductProfileForm form = new ProductProfileForm(filename);
			form.MdiParent = this;
			form.WindowState = FormWindowState.Maximized;
			form.Show();
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			ProductProfileForm form = this.ActiveMdiChild as ProductProfileForm;
			ProductProfile profile = form.ProductProfile;

			profile.Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProductProfileForm form = this.ActiveMdiChild as ProductProfileForm;
			ProductProfile profile = form.ProductProfile;

			profile.SaveAs();
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OptionsForm form = new OptionsForm();
			form.Owner = this;
			form.ShowDialog();
		}

		private void publishToolStripButton_Click(object sender, EventArgs e)
		{
			ProductProfileForm form = this.ActiveMdiChild as ProductProfileForm;
			ProductProfile profile = form.ProductProfile;
			profile.Save();

			Publisher publisher = new Publisher(profile);

			Cursor.Current = Cursors.WaitCursor;

			try
			{
				publisher.Publish();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ClickOnce Publisher");
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void MainForm_MdiChildActivate(object sender, EventArgs e)
		{
			UpdateButtons();			
		}

		private void UpdateButtons()
		{
			if (this.ActiveMdiChild == null)
			{
				saveAsToolStripMenuItem.Enabled = false;
				saveToolStripMenuItem.Enabled = false;
				saveToolStripButton.Enabled = false;
				publishToolStripMenuItem.Enabled = false;
				publishToolStripButton.Enabled = false;
			}
			else
			{
				saveAsToolStripMenuItem.Enabled = true;
				saveToolStripMenuItem.Enabled = true;
				saveToolStripButton.Enabled = true;
				publishToolStripMenuItem.Enabled = true;
				publishToolStripButton.Enabled = true;				
			}
		}
	}
}