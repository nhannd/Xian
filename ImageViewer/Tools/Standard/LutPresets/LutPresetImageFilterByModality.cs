using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Graphics;

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

		public override bool IsMatch(IPresentationImage image)
		{
			IImageSopProvider associatedDicom = image as IImageSopProvider;

			if (associatedDicom == null)
				return false;

			return (_modality == associatedDicom.ImageSop.Modality);
		}
	}
}
