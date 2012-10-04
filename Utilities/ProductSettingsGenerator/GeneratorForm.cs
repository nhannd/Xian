#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using ClearCanvas.Utilities.ProductSettingsGenerator.Properties;

namespace ClearCanvas.Utilities.ProductSettingsGenerator
{
	public partial class GeneratorForm : Form
	{
		public GeneratorForm()
		{
			InitializeComponent();

			_component.Text = ProductSettings.Default.Component;
			_product.Text = ProductSettings.Default.Product;
			_edition.Text = ProductSettings.Default.Edition;
			_release.Text = ProductSettings.Default.Release;
			_version.Text = ProductSettings.Default.Version;
			_versionSuffix.Text = ProductSettings.Default.VersionSuffix;
			_license.Text = ProductSettings.Default.License;
			_copyright.Text = ProductSettings.Default.Copyright;

			var standardSuffixes = new AutoCompleteStringCollection();
			foreach (var suffix in Settings.Default.StandardSuffixes)
				standardSuffixes.Add(suffix);
			_versionSuffix.AutoCompleteCustomSource = standardSuffixes;

			foreach (var release in Settings.Default.StandardReleases)
				_release.Items.Add(release);
		}

		private static void ShowFile(string filename)
		{
			try
			{
				Process.Start(Environment.ExpandEnvironmentVariables(Settings.Default.TextEditorCommand), Environment.ExpandEnvironmentVariables(string.Format(Settings.Default.TextEditorArgs, filename)));
			}
			catch (Exception)
			{
				Process.Start("explorer.exe", string.Format("/n,/select,{0}", filename));
			}
		}

		private void btnGenerateXml_Click(object sender, EventArgs e)
		{
			if (!ValidateFields())
				return;

			if (_dlgSaveXml.ShowDialog(this) != DialogResult.OK)
				return;

			try
			{
				var filename = _dlgSaveXml.FileName;
				var settings = new EncryptedProductSettings(
					_component.Text,
					_product.Text,
					_edition.Text,
					_release.Text,
					_version.Text,
					_versionSuffix.Text,
					_copyright.Text,
					_license.Text);
				settings.Save(filename);

				ShowFile(filename);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnGenerateConfig_Click(object sender, EventArgs e)
		{
			if (!ValidateFields())
				return;

			if (_dlgSaveConfig.ShowDialog(this) != DialogResult.OK)
				return;

			try
			{
				var filename = _dlgSaveConfig.FileName;
				var settings = new EncryptedProductSettings(
					_component.Text,
					_product.Text,
					_edition.Text,
					_release.Text,
					_version.Text,
					_versionSuffix.Text,
					_copyright.Text,
					_license.Text);
				var configuration = new ProductSettingsConfiguration(settings);
				configuration.Save(filename);

				ShowFile(filename);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void _btnReset_Click(object sender, EventArgs e)
		{
			_component.Text = GetDefaultSettingValue<ProductSettings>("Component");
			_product.Text = GetDefaultSettingValue<ProductSettings>("Product");
			_edition.Text = GetDefaultSettingValue<ProductSettings>("Edition");
			_release.Text = GetDefaultSettingValue<ProductSettings>("Release");
			_version.Text = GetDefaultSettingValue<ProductSettings>("Version");
			_versionSuffix.Text = GetDefaultSettingValue<ProductSettings>("VersionSuffix");
			_copyright.Text = GetDefaultSettingValue<ProductSettings>("Copyright");
			_license.Text = GetDefaultSettingValue<ProductSettings>("License");
		}

		private static TOutput TryConvert<TInput, TOutput>(TInput value, TOutput @default, Converter<TInput, TOutput> converter)
		{
			try
			{
				return converter.Invoke(value);
			}
			catch (Exception)
			{
				return @default;
			}
		}

		private static string GetDefaultSettingValue<T>(string name)
			where T : ApplicationSettingsBase
		{
			try
			{
				var @property = typeof (T).GetProperty(name);
				var attribute = (DefaultSettingValueAttribute) @property.GetCustomAttributes(typeof (DefaultSettingValueAttribute), false)[0];
				return attribute.Value;
			}
			catch (Exception)
			{
				throw new ArgumentException("The specified setting property does not exist.", "name");
			}
		}

		private bool ValidateFields()
		{
			var version = _version.Text;
			if (!string.IsNullOrEmpty(version))
			{
				try
				{
					new Version(version);
				}
				catch (Exception)
				{
					_version.Focus();
					_version.SelectAll();
					MessageBox.Show(this, "Version number must be in the form MAJOR.MINOR[.BUILD[.RELEASE]], with each component being a positive 32-bit integer.");
					return false;
				}
			}

			return true;
		}
	}
}