#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	[SettingsGroupDescription("A list of standard DICOM modalities that can be used anywhere a list of modalities is required.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class StandardModalitySettings : IMigrateSettings
	{
		private StandardModalitySettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		private static List<string> GetModalities(string modalities)
		{
			return CollectionUtils.Map(modalities.Split(','), (string s) => s.Trim());
		}

		private static string CombineModalities(string modalities1, string modalities2)
		{
			var combined = new SortedDictionary<string, string>();
			foreach (string modality in GetModalities(modalities1 ?? ""))
				combined[modality] = modality;
			foreach (string modality in GetModalities(modalities2 ?? ""))
				combined[modality] = modality;

			return StringUtilities.Combine(combined.Values, ",");
		}

		public List<string> GetModalities()
		{
			return GetModalities(Modalities);
		}

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			if (migrationValues.PropertyName != "Modalities")
				return;

			migrationValues.CurrentValue = CombineModalities(migrationValues.CurrentValue as string,
			                                                 migrationValues.PreviousValue as string);
	}

		#endregion
	}
}
