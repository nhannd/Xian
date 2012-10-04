#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClickOncePublisher
{
	public partial class OptionsForm : Form
	{
		private Options _options;

		public OptionsForm()
		{
			InitializeComponent();

			_options = new Options();

			_publishDirectory.DataBindings.Add("Text", _options, "PublishDirectory", true, DataSourceUpdateMode.OnPropertyChanged);
			_baseUrl.DataBindings.Add("Text", _options, "BaseUrl", true, DataSourceUpdateMode.OnPropertyChanged);
			_certificateFile.DataBindings.Add("Text", _options, "CertificateFile", true, DataSourceUpdateMode.OnPropertyChanged);
			_certificatePassword.DataBindings.Add("Text", _options, "CertificatePassword", true, DataSourceUpdateMode.OnPropertyChanged);
			_publishDirectoryBrowseButton.Click += OnBrowsePublishPath;
			_certificateFileBrowseButton.Click += OnBrowseCertificateFile;

			_okButton.Click += delegate
			                   	{
									_options.Save();
									Close();
			                   	};

			_cancelButton.Click += delegate
			                       	{
			                       		Close();
			                       	};
		}

		private void OnBrowsePublishPath(object sender, EventArgs e)
		{
			_options.PublishDirectory = CommonDialogHelper.GetPathFromFolderDialog(_options.PublishDirectory);
		}


		private void OnBrowseCertificateFile(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.ShowDialog();
			_options.CertificateFile = dlg.FileName;
		}

	}
}