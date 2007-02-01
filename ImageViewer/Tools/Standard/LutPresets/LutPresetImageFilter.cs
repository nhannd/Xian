using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	[XmlInclude(typeof(LutPresetImageFilterByModality))]
	public abstract class LutPresetImageFilter
	{
		public abstract bool IsMatch(IPresentationImage image);
	}
}
