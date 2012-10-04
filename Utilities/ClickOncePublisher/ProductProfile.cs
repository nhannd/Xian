#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using ClickOncePublisher.Properties;
using System.Text.RegularExpressions;

namespace ClickOncePublisher
{
	public class ProductProfile : INotifyPropertyChanged
	{
		private string _filename;
		private string _name;
		private string _version;
		private string _directory;
		private string _entryPointPath;
		private List<string> _prerequisites = new List<string>();
		private string _applicationUrl;
		private string _setupUrl;
		private bool _validationPassed;
		private event EventHandler _saving;

		private bool _dirty;

		public event PropertyChangedEventHandler PropertyChanged;

		public ProductProfile()
		{

		}

		public ProductProfile(string filename)
		{
			if (filename != null)
			{
				Filename = filename;

				XmlSerializer serializer = new XmlSerializer(typeof (ProductProfile));
				FileStream fs = new FileStream(filename, FileMode.Open);
				ProductProfile profile = (ProductProfile) serializer.Deserialize(fs);
				fs.Close();

				_name = profile.Name;
				_version = profile.Version;
				_directory = profile.Directory;
				_entryPointPath = profile.EntryPointPath;
				_prerequisites = profile.Prerequisites;

				Dirty = false;
			}

			UpdateUrls();
			Settings.Default.PropertyChanged += OnSettingsChanged;
		}


		#region Properties

		[XmlIgnore]
		public string Filename
		{
			get { return _filename; }
			set
			{
				if (_filename != value)
				{
					_filename = value;
					NotifyPropertyChanged("Filename");
				}
			}
			
		}

		[XmlIgnore]
		public string SuggestedFilename
		{
			get
			{
				string str = String.Format("{0}_{1}.xml", Name, Version);
				return str.Replace(' ', '_');
			}
		}

		[XmlIgnore]
		public bool Dirty
		{
			get { return _dirty; }
			set
			{
				if (_dirty != value)
				{
					_dirty = value;
					NotifyPropertyChanged("Dirty");
				}
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					NotifyPropertyChanged("Name");
					Dirty = true;
					UpdateUrls();
				}
			}
		}

		public string Version
		{
			get { return _version; }
			set
			{
				if (_version != value)
				{
					_version = value;
					NotifyPropertyChanged("Version");
					Dirty = true;
				}
			}
		}

		public string Directory
		{
			get { return _directory; }
			set
			{
				if (_directory != value)
				{
					_directory = value;
					NotifyPropertyChanged("Directory");
					Dirty = true;
				}
			}
		}

		public string EntryPointPath
		{
			get { return _entryPointPath; }
			set
			{
				if (_entryPointPath != value)
				{
					_entryPointPath = value;
					NotifyPropertyChanged("EntryPointPath");
					Dirty = true;
				}
			}
		}

		public List<string> Prerequisites
		{
			get { return _prerequisites; }
			set
			{
				_prerequisites = value;
				NotifyPropertyChanged("Prerequisites");
				Dirty = true;
			}
		}

		[XmlIgnore]
		public string ApplicationUrl
		{
			get { return _applicationUrl; }
			set
			{
				if (_applicationUrl != value)
				{
					_applicationUrl = value;
					NotifyPropertyChanged("ApplicationUrl");
				}
			}			
		}

		[XmlIgnore]
		public string SetupUrl
		{
			get { return _setupUrl; }
			set
			{
				if (_setupUrl != value)
				{
					_setupUrl = value;
					NotifyPropertyChanged("SetupUrl");
				}
			}
		}

		#endregion

		public event EventHandler Saving
		{
			add { _saving += value; }
			remove { _saving -= value; }
		}

		public bool Save()
		{
			if (String.IsNullOrEmpty(Filename))
				return SaveAs();
			else
			{
				_validationPassed = true;
				_saving.Invoke(this, EventArgs.Empty);

				return SaveAs(Filename);
			}
		}

		public bool SaveAs()
		{
			_validationPassed = true;
			_saving.Invoke(this, EventArgs.Empty);

			if (!_validationPassed)
				return false;

			string filename = CommonDialogHelper.GetSaveFilename(SuggestedFilename);

			if (String.IsNullOrEmpty(filename))
				return false;

			return SaveAs(filename);
		}

		private bool SaveAs(string filename)
		{
			if (!_validationPassed)
				return false;

			XmlSerializer serializer = new XmlSerializer(typeof(ProductProfile));
			TextWriter writer = new StreamWriter(filename);
			serializer.Serialize(writer, this);
			writer.Close();

			Filename = filename;
			Dirty = false;

			return true;
		}

		public void UpdateUrls()
		{
			string str = String.Format("{0}/{1}/{2}.application", Settings.Default.BaseUrl, Name, Name);
			ApplicationUrl = str.Replace(' ', '_');
			str = String.Format("{0}/{1}/setup.exe", Settings.Default.BaseUrl, Name);
			SetupUrl = str.Replace(' ', '_');
		}

		public string ValidateName()
		{
			if (String.IsNullOrEmpty(Name))
			{
				_validationPassed = false;
				return "Name cannot be empty";
			}

			Regex regex = new Regex(@"[^a-zA-Z0-9' ''.']");

			if (regex.IsMatch(Name))
			{
				_validationPassed = false;
				return "Name must be alphanumeric";
			}

			_validationPassed &= true;
			return "";
		}

		public string ValidateVersion()
		{
			if (String.IsNullOrEmpty(Version))
			{
				_validationPassed = false;
				return "Version cannot be empty";
			}

			Regex regex = new Regex(@"\b\d{1,5}\.\d{1,5}\.\d{1,5}\.\d{1,5}\b");

			if (!regex.IsMatch(Version))
			{
				_validationPassed = false;
				return "Version must in the form a.b.c.d";
			}

			_validationPassed &= true;
			return "";
		}

		public string ValidateDirectory()
		{
			if (String.IsNullOrEmpty(Directory))
			{
				_validationPassed = false;
				return "Directory cannot be empty";
			}

			_validationPassed &= true;
			return "";
		}

		public string ValidateEntryPointPath()
		{
			if (String.IsNullOrEmpty(EntryPointPath))
			{
				_validationPassed = false;
				return "Entry point path cannot be empty";
			}

			_validationPassed &= true;
			return "";
		}

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateUrls();
		}
	}
}
