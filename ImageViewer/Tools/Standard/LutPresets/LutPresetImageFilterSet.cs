using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class LutPresetImageFilterSet
	{
		private List<LutPresetImageFilter> _filters;

		public LutPresetImageFilterSet()
		{
		}

		public List<LutPresetImageFilter> Filters
		{
			get { return _filters; }
			set { _filters = value; }
		}

		public bool IsMatch(IPresentationImage image)
		{
			if (image == null)
				return false;

			foreach (LutPresetImageFilter filter in _filters)
			{
				if (!filter.IsMatch(image))
					return false;
			}

			return true;
		}
	}
}
