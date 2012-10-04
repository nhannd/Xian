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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Common
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	internal class DumpProductSettingsApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Dump();
		}

		private void Dump()
		{
			var settings = new DecryptedProductSettings();
			Console.WriteLine("Product: {0}", settings.Product);
			Console.WriteLine("Component: {0}", settings.Component);
			Console.WriteLine("Edition: {0}", settings.Edition);
			Console.WriteLine("Version: {0}", settings.Version);
			Console.WriteLine("VersionSuffix: {0}", settings.VersionSuffix);
			Console.WriteLine("Release: {0}", settings.Release);
			Console.WriteLine("Copyright:\n{0}", settings.Copyright);
			Console.WriteLine("\nLicense:\n{0}", settings.License);
		}

		#endregion
	}

	/// <summary>
	/// Provides some basic information about the product, such as the name and version.
	/// </summary>
	internal class DecryptedProductSettings
	{
		private string _product;
		private string _component;
		private string _edition;
		private string _release;
		private Version _version;
		private string _versionSuffix;
		private string _copyright;
		private string _license;

		private readonly ProductSettings _settings;

		public DecryptedProductSettings()
		{
			_settings = new ProductSettings();
			_settings.PropertyChanged += OnSettingPropertyChanged;
		}

		private void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_product = null;
			_component = null;
			_edition = null;
			_release = null;
			_version = null;
			_versionSuffix = null;
			_copyright = null;
			_license = null;
		}

		/// <summary>
		/// Gets the component name.
		/// </summary>
		public string Component
		{
			get
			{
				if (_component == null)
					_component = Decrypt(_settings.Component);
				return _component;
			}
		}

		/// <summary>
		/// Gets the product name.
		/// </summary>
		public string Product
		{
			get
			{
				if (_product == null)
					_product = Decrypt(_settings.Product);
				return _product;
			}
		}

		/// <summary>
		/// Gets the product edition.
		/// </summary>
		public string Edition
		{
			get
			{
				if (_edition == null)
					_edition = Decrypt(_settings.Edition);
				return _edition;
			}
		}

		/// <summary>
		/// Gets the product release type.
		/// </summary>
		public string Release
		{
			get
			{
				if (_release == null)
				{
					var release = Decrypt(_settings.Release);
					_release = string.IsNullOrEmpty(release) || release[0] != '*' ? "Unverified" : release.Substring(1);
				}
				return _release;
			}
		}

		/// <summary>
		/// Gets the product version.
		/// </summary>
		public Version Version
		{
			get
			{
				if (_version == null)
				{
					string version = Decrypt(_settings.Version);
					try
					{
						if (String.IsNullOrEmpty(version))
							_version = Assembly.GetExecutingAssembly().GetName().Version;
						else
							_version = new Version(version);
					}
					catch (Exception)
					{
						// don't allow a poorly formatted version string to kill the app
						_version = Assembly.GetExecutingAssembly().GetName().Version;
					}
				}
				return _version;
			}
		}

		/// <summary>
		/// Gets the product version suffix (e.g. "SP1").
		/// </summary>
		public string VersionSuffix
		{
			get
			{
				if (_versionSuffix == null)
				{
					string versionSuffix = Decrypt(_settings.VersionSuffix);
					if (String.IsNullOrEmpty(versionSuffix) || versionSuffix[0] != '*')
						_versionSuffix = "Unverified Build";
					else
						_versionSuffix = versionSuffix.Substring(1);
				}
				return _versionSuffix;
			}
		}

		/// <summary>
		/// Gets the product copyright (e.g. "Copyright 2009 ClearCanvas Inc.").
		/// </summary>
		public string Copyright
		{
			get
			{
				if (_copyright == null)
					_copyright = Decrypt(_settings.Copyright);
				return _copyright;
			}
		}

		/// <summary>
		/// Gets the product license.
		/// </summary>
		public string License
		{
			get
			{
				if (_license == null)
					_license = Decrypt(_settings.License);
				return _license;
			}
		}

		private static string Decrypt(string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return @string;

			string result;
			try
			{
				byte[] bytes = Convert.FromBase64String(@string);
				using (MemoryStream dataStream = new MemoryStream(bytes))
				{
					RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider();
					cryptoService.Key = Encoding.UTF8.GetBytes("ClearCanvas");
					cryptoService.IV = Encoding.UTF8.GetBytes("IsSoCool");
					cryptoService.UseSalt = false;
					using (CryptoStream cryptoStream = new CryptoStream(dataStream, cryptoService.CreateDecryptor(), CryptoStreamMode.Read))
					{
						using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
						{
							result = reader.ReadToEnd();
							reader.Close();
						}
						cryptoStream.Close();
					}
					dataStream.Close();
				}
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}
	}
	
	public static class ProductInformation
	{
		private static readonly DecryptedProductSettings _settings = new DecryptedProductSettings();

		/// <summary>
		/// Gets the component name.
		/// </summary>
		[Obsolete("It is recommended that the Component property be used to avoid ambiguity.")]
		public static string Name
		{
			get { return _settings.Component; }
		}

		/// <summary>
		/// Gets the component name.
		/// </summary>
		public static string Component
		{
			get { return _settings.Component; }
		}

		/// <summary>
		/// Gets the product name.
		/// </summary>
		public static string Product
		{
			get { return _settings.Product; }
		}

		/// <summary>
		/// Gets the product edition.
		/// </summary>
		public static string Edition
		{
			get { return _settings.Edition; }
		}

		/// <summary>
		/// Gets the product release type.
		/// </summary>
		public static string Release
		{
			get { return _settings.Release; }
		}

		/// <summary>
		/// Gets the product version.
		/// </summary>
		public static Version Version
		{
			get { return _settings.Version; }
		}

		/// <summary>
		/// Gets the product version suffix (e.g. "SP1").
		/// </summary>
		public static string VersionSuffix
		{
			get { return _settings.VersionSuffix; }
		}

		/// <summary>
		/// Gets the product copyright (e.g. "Copyright 2009 ClearCanvas Inc.").
		/// </summary>
		public static string Copyright
		{
			get { return _settings.Copyright; }
		}

		/// <summary>
		/// Gets the product license.
		/// </summary>
		public static string License
		{
			get { return _settings.License; }
		}

		/// <summary>
		/// Gets the component name, optionally with the product edition and/or release type.
		/// </summary>
		/// <param name="includeEdition">A value indciating whether or not to include the product edition in the name.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		public static string GetName(bool includeEdition, bool includeRelease)
		{
			var sb = new StringBuilder(Component);
			if (includeEdition && !string.IsNullOrEmpty(Edition))
			{
				if (sb.Length > 0)
					sb.Append(' ');
				sb.Append(Edition);
			}
			if (includeRelease && !string.IsNullOrEmpty(Release))
			{
				if (sb.Length > 0)
					sb.Append(' ');
				sb.AppendFormat("({0})", Release);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Gets a string containing both the component name, product edition, release type and version.
		/// </summary>
		/// <param name="includeBuildAndRevision">A value indicating whether or not to include the build and revision numbers in the version; False means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">A value indicating whether or not to include the version suffix.</param>
		public static string GetNameAndVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			return GetNameAndVersion(includeBuildAndRevision, includeVersionSuffix, true, true);
		}

		/// <summary>
		/// Gets a string containing both the component name and version, optionally with the product edition and/or release type.
		/// </summary>
		/// <param name="includeBuildAndRevision">A value indicating whether or not to include the build and revision numbers in the version; False means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">A value indicating whether or not to include the version suffix.</param>
		/// <param name="includeEdition">A value indciating whether or not to include the product edition in the name.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		public static string GetNameAndVersion(bool includeBuildAndRevision, bool includeVersionSuffix, bool includeEdition, bool includeRelease)
		{
			return string.Format("{0} {1}", GetName(includeEdition, includeRelease), GetVersion(includeBuildAndRevision, includeVersionSuffix, false));
		}

		/// <summary>
		/// Gets the version as a string, optionally with build and revision numbers, and/or version suffix.
		/// </summary>
		/// <param name="includeBuildAndRevision">Specifies whether to include the build and revision numbers in the version; false means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">Specifies whether to include the version suffix.</param>
		public static string GetVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			return GetVersion(includeBuildAndRevision, includeVersionSuffix, false);
		}

		/// <summary>
		/// Gets the version as a string, optionally with build and revision numbers, version suffix, and/or release type.
		/// </summary>
		/// <param name="includeBuildAndRevision">Specifies whether to include the build and revision numbers in the version; false means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">Specifies whether to include the version suffix.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		public static string GetVersion(bool includeBuildAndRevision, bool includeVersionSuffix, bool includeRelease)
		{
			var version = Version;
			var versionString = new StringBuilder(string.Format("{0}.{1}", version.Major, version.Minor));

			if (includeBuildAndRevision && version.Build >= 0)
			{
				versionString.AppendFormat(".{0}", version.Build);
				if (version.Revision >= 0)
					versionString.AppendFormat(".{0}", version.Revision);
			}

			if (includeVersionSuffix && !string.IsNullOrEmpty(VersionSuffix))
				versionString.Append(" " + VersionSuffix);

			if (includeRelease && !string.IsNullOrEmpty(Release))
				versionString.Append(" " + Release);

			return versionString.ToString();
		}
	}

	[SettingsGroupDescription("Settings that describe the product, such as the product name and version.")]
	[SettingsProvider(typeof (ApplicationCriticalSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class ProductSettings
	{
		internal ProductSettings() {}
	}
}