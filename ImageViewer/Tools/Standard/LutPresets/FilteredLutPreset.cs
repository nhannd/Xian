using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class FilteredLutPreset
	{
		private LutPresetImageFilterSet _filterSet;
		private LutPreset _preset;

		public FilteredLutPreset()
		{
		}

		public LutPresetImageFilterSet FilterSet
		{
			get { return _filterSet; }
			set { _filterSet = value; }
		}

		public LutPreset Preset
		{
			get { return _preset; }
			set { _preset = value; }
		}

		public bool IsMatch(DicomPresentationImage image)
		{
			if (image == null)
				return false;
			
			return _filterSet.IsMatch(image);
		}

		public bool Apply(DicomPresentationImage image)
		{
			if (!_filterSet.IsMatch(image))
				return false;

			return _preset.Apply(image);
		}
	}
}
