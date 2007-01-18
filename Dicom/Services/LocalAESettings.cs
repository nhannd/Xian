#if !MONO

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.Dicom.Services
{
	public class LocalAESettings : ExtensionSettings
	{
		public LocalAESettings()
		{

		}

		[ApplicationScopedSettingAttribute()]
		[DefaultSettingValueAttribute("AETITLE")]
		public string AETitle
		{
			get { return (String)this["AETitle"]; }
			set { this["AETitle"] = value; }
		}

		[ApplicationScopedSettingAttribute()]
		[DefaultSettingValueAttribute("4000")]
		public int Port
		{
			get { return (int)this["Port"]; }
			set { this["Port"] = value; }
		}

		[ApplicationScopedSettingAttribute()]
		[DefaultSettingValueAttribute(".\\dicom")]
		public string DicomStoragePath
		{
			get { return (string)this["DicomStoragePath"]; }
			set { this["DicomStoragePath"] = value; }
		}
	}
}

#endif