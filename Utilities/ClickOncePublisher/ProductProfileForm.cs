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
using System.Windows.Forms;

namespace ClickOncePublisher
{
	public partial class ProductProfileForm : Form
	{
		private readonly ProductProfile _productProfile;

		public ProductProfileForm() : this(null)
		{
			Text = "Product Profile";
		}

		public ProductProfileForm(string filename)
		{
			InitializeComponent();
			
			_nameErrorProvider.SetIconAlignment(_name, ErrorIconAlignment.MiddleRight);
			_versionErrorProvider.SetIconAlignment(_version, ErrorIconAlignment.MiddleRight);
			_directoryErrorProvider.SetIconAlignment(_directory, ErrorIconAlignment.MiddleRight);
			_entryPointPathErrorProvider.SetIconAlignment(_entryPointPath, ErrorIconAlignment.MiddleRight);

			_productProfile = new ProductProfile(filename);
			_name.DataBindings.Add("Text", _productProfile, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_version.DataBindings.Add("Text", _productProfile, "Version", true, DataSourceUpdateMode.OnPropertyChanged);
			_directory.DataBindings.Add("Text", _productProfile, "Directory", true, DataSourceUpdateMode.OnPropertyChanged);
			_entryPointPath.DataBindings.Add("Text", _productProfile, "EntryPointPath", true, DataSourceUpdateMode.OnPropertyChanged);
			_applicationUrl.DataBindings.Add("Text", _productProfile, "ApplicationUrl", true, DataSourceUpdateMode.OnPropertyChanged);
			_setupUrl.DataBindings.Add("Text", _productProfile, "SetupUrl", true, DataSourceUpdateMode.OnPropertyChanged);

			InitializeListBox();

			_productDirectoryBrowseButton.Click += OnBrowseProductPath;
			_entryPointPathBrowseButton.Click += OnBrowseEntryPointPath;
			ProductProfile.PropertyChanged += OnProductProfilePropertyChanged;
			ProductProfile.Saving += OnSaving;
			UpdateDirtyStatus();
		}

		public ProductProfile ProductProfile
		{
			get { return _productProfile; }
		}

		private void OnSaving(object sender, EventArgs e)
		{
			ValidateChildren();
		}

		private void UpdateDirtyStatus()
		{
			if (ProductProfile.Filename == null)
				return;

			string filename = Path.GetFileName(ProductProfile.Filename);

			Text = filename;

			if (_productProfile.Dirty)
				Text = filename + "*";			
		}

		private void OnPackagesItemCheck(object sender, ItemCheckEventArgs e)
		{
			string package = _packages.Items[e.Index].ToString();

			if (e.NewValue == CheckState.Checked)
			{
				if (!ProductProfile.Prerequisites.Contains(package))
				{
					ProductProfile.Prerequisites.Add(package);
					ProductProfile.Dirty = true;
				}
			}
			else
			{
				ProductProfile.Prerequisites.Remove(package);
				ProductProfile.Dirty = true;
			}
		}

		private void OnProductProfilePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Dirty")
				UpdateDirtyStatus();
		}

		private void OnBrowseProductPath(object sender, EventArgs e)
		{
			ProductProfile.Directory = CommonDialogHelper.GetPathFromFolderDialog(_productProfile.Directory);
			OnDirectoryValidated(null, null);
		}

		private void OnBrowseEntryPointPath(object sender, EventArgs e)
		{
			ProductProfile.EntryPointPath = CommonDialogHelper.GetExeFilename(_productProfile.Directory);
			OnEntryPointPathValidated(null, null);
		}

		private void InitializeListBox()
		{
			_packages.ItemCheck += OnPackagesItemCheck;

			// Populate the list box
			List<string> packages = PackageEnumerator.GetPackages(@".\Packages");

			foreach (string package in packages)
				_packages.Items.Add(package);

			// Check the selected packages
			foreach (string selectedPackage in ProductProfile.Prerequisites)
			{
				int index = _packages.FindStringExact(selectedPackage);

				if (index == -1)
					continue;

				_packages.SetItemChecked(index, true);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (ProductProfile.Dirty)
			{
				DialogResult result = MessageBox.Show("Save file?", "ClickOnce Publisher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (result == DialogResult.Yes)
					if (!ProductProfile.Save())
						e.Cancel = true;
			}
			base.OnClosing(e);
		}

		private void OnNameValidated(object sender, EventArgs e)
		{
			_nameErrorProvider.SetError(_name, ProductProfile.ValidateName());
		}

		private void OnVersionValidated(object sender, EventArgs e)
		{
			_versionErrorProvider.SetError(_version, ProductProfile.ValidateVersion());
		}

		private void OnDirectoryValidated(object sender, EventArgs e)
		{
			_directoryErrorProvider.SetError(_directory, ProductProfile.ValidateDirectory());
		}

		private void OnEntryPointPathValidated(object sender, EventArgs e)
		{
			_entryPointPathErrorProvider.SetError(_entryPointPath, ProductProfile.ValidateEntryPointPath());
		}

	}
}