using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public static class VoiLutPresetConfigurations
	{
		public static VoiLutPresetConfigurationCollection GetVoiLutPresetConfigurations()
		{
			return VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations();
		}

		public static IDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByKeyStroke()
		{
			return VoiLutPresetConfigurationsHelper.GetConfigurationsByKeyStroke(VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations());
		}

		public static IDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByKeyStroke(string voiLutFactoryKey)
		{
			return VoiLutPresetConfigurationsHelper.GetConfigurationsByKeyStroke(VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations(), voiLutFactoryKey);
		}

		public static IDictionary<string, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByModality()
		{
			return VoiLutPresetConfigurationsHelper.GetConfigurationsByModality(VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations());
		}

		public static IDictionary<string, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByModality(string voiLutFactoryKey)
		{
			return VoiLutPresetConfigurationsHelper.GetConfigurationsByModality(VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations(), voiLutFactoryKey);
		}
	}

	public static class VoiLutPresetConfigurationsHelper
	{
		private class SimpleXKeysSorter : IComparer<XKeys>
		{
			public SimpleXKeysSorter()
			{
			}
			#region IComparer<XKeys> Members

			public int Compare(XKeys x, XKeys y)
			{
				if (x == XKeys.None)
				{
					if (y != XKeys.None)
						return 1;
				}
				else if (y == XKeys.None)
					return -1;

				return Math.Sign(x - y);
			}

			#endregion
		}

		public static IDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByKeyStroke(IEnumerable<VoiLutPresetConfiguration> configurations)
		{
			return GetConfigurationsByKeyStroke(configurations, null);
		}

		public static IDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByKeyStroke(IEnumerable<VoiLutPresetConfiguration> configurations, string voiLutFactoryKey)
		{
			SortedDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>> configurationsByKeyStroke =
				new SortedDictionary<XKeys, IEnumerable<VoiLutPresetConfiguration>>(new SimpleXKeysSorter());

			foreach (VoiLutPresetConfiguration presetConfiguration in configurations)
			{
				if (!configurationsByKeyStroke.ContainsKey(presetConfiguration.KeyStroke))
					configurationsByKeyStroke.Add(presetConfiguration.KeyStroke, new List<VoiLutPresetConfiguration>());

				if (!String.IsNullOrEmpty(voiLutFactoryKey) && presetConfiguration.VoiLutPresetApplicatorConfiguration.FactoryKey != voiLutFactoryKey)
					continue;

				((List<VoiLutPresetConfiguration>)configurationsByKeyStroke[presetConfiguration.KeyStroke]).Add(presetConfiguration);
			}

			return configurationsByKeyStroke;
		}

		public static IDictionary<string, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByModality(IEnumerable<VoiLutPresetConfiguration> configurations)
		{
			return GetConfigurationsByModality(configurations, null);
		}

		public static IDictionary<string, IEnumerable<VoiLutPresetConfiguration>> GetConfigurationsByModality(IEnumerable<VoiLutPresetConfiguration> configurations, string voiLutFactoryKey)
		{
			Dictionary<string, IEnumerable<VoiLutPresetConfiguration>> configurationsByModality = new Dictionary<string, IEnumerable<VoiLutPresetConfiguration>>();
			foreach (VoiLutPresetConfiguration presetConfiguration in configurations)
			{
				if (!configurationsByModality.ContainsKey(presetConfiguration.ModalityFilter))
					configurationsByModality.Add(presetConfiguration.ModalityFilter, new List<VoiLutPresetConfiguration>());

				if (!String.IsNullOrEmpty(voiLutFactoryKey) && presetConfiguration.VoiLutPresetApplicatorConfiguration.FactoryKey != voiLutFactoryKey)
					continue;

				((List<VoiLutPresetConfiguration>)configurationsByModality[presetConfiguration.ModalityFilter]).Add(presetConfiguration);
			}

			return configurationsByModality;
		}
	}
}
