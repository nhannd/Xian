using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class LutPresetImageFilterByModality : LutPresetImageFilter
	{
		private string _modality;

		public string Modality
		{
			get { return _modality; }
			set { _modality = value; }
		}
	
		public override bool IsMatch(DicomPresentationImage image)
		{
			return (_modality == image.ImageSop.Modality);
		}
	}
}
