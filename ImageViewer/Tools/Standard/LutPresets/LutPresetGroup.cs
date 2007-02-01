using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;
using System.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class LutPresetGroup
	{
		private string _actionId;
		private List<FilteredLutPreset> _filteredPresets;

		public LutPresetGroup()
		{
		}

		public List<FilteredLutPreset> FilteredPresets
		{
			get { return _filteredPresets; }
			set { _filteredPresets = value; }
		}

		public string ActionId
		{
			get { return _actionId; }
			set { _actionId = value; }
		}

		public FilteredLutPreset GetFirstMatch(StandardPresentationImage image)
		{
			if (image == null)
				return null;

			foreach (FilteredLutPreset filteredPreset in _filteredPresets)
			{
				if (filteredPreset.IsMatch(image))
					return filteredPreset;
			}

			return null;
		}

		public bool Apply(StandardPresentationImage image)
		{
			if (image == null)
				return false;

			foreach (FilteredLutPreset filteredPreset in _filteredPresets)
			{
				if (filteredPreset.Apply(image))
					return true;
			}

			return false;
		}

	}
}
