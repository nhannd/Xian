#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
