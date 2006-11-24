using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	[XmlInclude(typeof(NamedVoiLutPreset))]
	[XmlInclude(typeof(VoiLutPreset))]
	public abstract class LutPreset
	{
		public abstract string Label { get; set; }
		public abstract bool Apply(DicomPresentationImage image);
	}
}
