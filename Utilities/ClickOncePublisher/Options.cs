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
using System.IO;
using System.Text;
using ClickOncePublisher.Properties;

namespace ClickOncePublisher
{
	class Options : INotifyPropertyChanged
	{
		private string _publishDirectory;
		private string _certificateFile;
		private string _certificatePassword;
		private string _baseUrl;

		public event PropertyChangedEventHandler PropertyChanged;

		public Options()
		{
			if (Directory.Exists(Settings.Default.PublishDirectory))
				PublishDirectory = Settings.Default.PublishDirectory;
			else
				PublishDirectory = "C:\\";

			if (File.Exists(Settings.Default.CertificateFile))
				CertificateFile = Settings.Default.CertificateFile;
			else
				CertificateFile = "C:\\";

			CertificatePassword = Settings.Default.CertificatePassword;
			BaseUrl = Settings.Default.BaseUrl;
		}

		#region Public Properties

		public string PublishDirectory
		{
			get { return _publishDirectory; }
			set 
			{
				if (_publishDirectory != value)
				{
					_publishDirectory = value;
					NotifyPropertyChanged("PublishDirectory");
				}
			}
		}

		public string BaseUrl
		{
			get { return _baseUrl; }
			set 
			{
				if (_baseUrl != value)
				{
					_baseUrl = value;
					NotifyPropertyChanged("BaseUrl");
				}
			}
		}


		public string CertificateFile
		{
			get { return _certificateFile; }
			set
			{
				if (_certificateFile != value)
				{
					_certificateFile = value;
					NotifyPropertyChanged("CertificateFile");
				}
			}
		}

		public string CertificatePassword
		{
			get { return _certificatePassword; }
			set
			{
				if (_certificatePassword != value)
				{
					_certificatePassword = value;
					NotifyPropertyChanged("CertificatePassword");
				}
			}
		}

		#endregion

		public void Save()
		{
			Settings.Default.PublishDirectory = PublishDirectory;
			Settings.Default.CertificateFile = CertificateFile;
			Settings.Default.CertificatePassword = CertificatePassword;
			Settings.Default.BaseUrl = BaseUrl;
			Settings.Default.Save();
		}

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
