#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ClearCanvas.Common
{
	public static class ProductInformation
	{
		private static string _name;
		private static Version _version;
		private static string _versionSuffix;
		private static string _copyright;
		private static string _license;

		static ProductInformation()
		{
			ProductSettings.Default.PropertyChanged += OnSettingPropertyChanged;
		}

		static void OnSettingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_name = null;
			_version = null;
			_versionSuffix = null;
			_copyright = null;
			_license = null;
		}

		public static string Name
		{
			get
			{
				if (_name == null)
					_name = Decrypt(ProductSettings.Default.Name);

				return _name;
			}
		}

		public static Version Version
		{
			get
			{
				if (_version == null)
				{
					string version = Decrypt(ProductSettings.Default.Version);
					if (String.IsNullOrEmpty(version))
						_version = Assembly.GetExecutingAssembly().GetName().Version;
                    else
						_version = new Version(version);
				}

				return _version;
			}
		}

		public static string VersionSuffix
		{
			get
			{
				if (_versionSuffix == null)
					_versionSuffix = Decrypt(ProductSettings.Default.VersionSuffix);

				return _versionSuffix;
			}
		}

		public static string Copyright
		{
			get
			{
				if (_copyright == null)
					_copyright = Decrypt(ProductSettings.Default.Copyright);

				return _copyright;
			}
		}

		public static string License
		{
			get
			{
				if (_license == null)
					_license = Decrypt(ProductSettings.Default.License);

				return _license;
			}
		}

		private static string Decrypt(string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return @string;

			byte[] bytes = Convert.FromBase64String(@string);
			return Encoding.UTF8.GetString(bytes);
		}

		public static string GetNameAndVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			return String.Format("{0} {1}", Name, GetVersion(includeBuildAndRevision, includeVersionSuffix));
		}

		public static string GetVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			string versionString;
			Version version = Version;

			if (includeBuildAndRevision)
				versionString = String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
			else
				versionString = String.Format("{0}.{1}", version.Major, version.Minor);

			if (includeVersionSuffix && !String.IsNullOrEmpty(VersionSuffix))
				return String.Format("{0} {1}", versionString, VersionSuffix);

			return versionString;
		}
	}

	[SettingsGroupDescription("Settings that describe the product, such as the product name and version.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed partial class ProductSettings
	{
		private ProductSettings()
		{
		}
	}
}
