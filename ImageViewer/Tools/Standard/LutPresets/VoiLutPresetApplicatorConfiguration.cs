using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public sealed class VoiLutPresetApplicatorConfiguration
	{
		private string _factoryKey;
		private IDictionary<string, string> _configurationValues;

		public VoiLutPresetApplicatorConfiguration(string factoryKey, IDictionary<string, string> configurationValues)
		{
			Platform.CheckForEmptyString(factoryKey, "factoryKey");
			Platform.CheckForNullReference(configurationValues, "configurationValues");
			
			_factoryKey = factoryKey;
			_configurationValues = configurationValues;
		}

		private VoiLutPresetApplicatorConfiguration()
		{ 
		}

		public string FactoryKey
		{
			get { return _factoryKey; }
		}

		public IDictionary<string, string> ConfigurationValues
		{
			get { return _configurationValues; }
		}
	}
}
