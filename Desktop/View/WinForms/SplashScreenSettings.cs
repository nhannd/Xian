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

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Drawing;

using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
	public sealed class SplashScreenSettingsHelper
	{
		/// <summary>
		/// Gets the default instance of the settings helper.
		/// </summary>
		public static readonly SplashScreenSettingsHelper Default = new SplashScreenSettingsHelper(SplashScreenSettings.Default);

		private readonly SplashScreenSettings _settings;

		internal SplashScreenSettingsHelper(SplashScreenSettings settings)
		{
			_settings = settings;
		}

		public bool UseSplashScreenSettings
		{
			get { return _settings.UseSplashScreenSettings; }
		}

		public string BackgroundImageAssemblyName
		{
			get { return _settings.BackgroundImageAssemblyName; }
		}

		public string BackgroundImageResourceName
		{
			get { return _settings.BackgroundImageResourceName; }
		}

		public bool StatusVisible
		{
			get { return _settings.StatusVisible; }
		}

		public Point StatusLocation
		{
			get { return _settings.StatusLocation; }
		}

		public Size StatusSize
		{
			get { return _settings.StatusSize; }
		}

		public bool StatusAutoSize
		{
			get { return _settings.StatusAutoSize; }
		}

		public Color StatusForeColor
		{
			get { return _settings.StatusForeColor; }
		}

		public bool StatusFontBold
		{
			get { return _settings.StatusFontBold; }
		}

		public ContentAlignment StatusTextAlign
		{
			get { return _settings.StatusTextAlign; }
		}

		public bool CopyrightVisible
		{
			get { return _settings.CopyrightVisible; }
		}

		public Point CopyrightLocation
		{
			get { return _settings.CopyrightLocation; }
		}

		public Size CopyrightSize
		{
			get { return _settings.CopyrightSize; }
		}

		public bool CopyrightAutoSize
		{
			get { return _settings.CopyrightAutoSize; }
		}

		public Color CopyrightForeColor
		{
			get { return _settings.CopyrightForeColor; }
		}

		public bool CopyrightFontBold
		{
			get { return _settings.CopyrightFontBold; }
		}

		public ContentAlignment CopyrightTextAlign
		{
			get { return _settings.CopyrightTextAlign; }
		}

		public string CopyrightText
		{
			get { return _settings.CopyrightText; }
		}

		public bool VersionVisible
		{
			get { return _settings.VersionVisible; }
		}

		public Point VersionLocation
		{
			get { return _settings.VersionLocation; }
		}

		public Size VersionSize
		{
			get { return _settings.VersionSize; }
		}

		public bool VersionAutoSize
		{
			get { return _settings.VersionAutoSize; }
		}

		public Color VersionForeColor
		{
			get { return _settings.VersionForeColor; }
		}

		public bool VersionFontBold
		{
			get { return _settings.VersionFontBold; }
		}

		public ContentAlignment VersionTextAlign
		{
			get { return _settings.VersionTextAlign; }
		}

		public string VersionTextFormat
		{
			get { return _settings.VersionTextFormat; }
		}

		public string VersionAssemblyName
		{
			get { return _settings.VersionAssemblyName; }
		}

		public bool LicenseVisible
		{
			get { return _settings.LicenseVisible; }
		}

		public Point LicenseLocation
		{
			get { return _settings.LicenseLocation; }
		}

		public Size LicenseSize
		{
			get { return _settings.LicenseSize; }
		}

		public bool LicenseAutoSize
		{
			get { return _settings.LicenseAutoSize; }
		}

		public Color LicenseForeColor
		{
			get { return _settings.LicenseForeColor; }
		}

		public bool LicenseFontBold
		{
			get { return _settings.LicenseFontBold; }
		}

		public ContentAlignment LicenseTextAlign
		{
			get { return _settings.LicenseTextAlign; }
		}

		public string LicenseText
		{
			get { return _settings.LicenseText; }
		}

		public bool PluginIconsVisible
		{
			get { return _settings.PluginIconsVisible; }
		}

		public Rectangle PluginIconsRectangle
		{
			get { return _settings.PluginIconsRectangle; }
		}
	}

	[SettingsGroupDescription("Stores splash screen settings in a common place")]
	internal sealed partial class SplashScreenSettings
	{
		private SplashScreenSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
