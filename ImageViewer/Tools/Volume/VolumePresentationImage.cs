using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImage : PresentationImage
	{
		public VolumePresentationImage()
		{

		}

		public override IRenderer ImageRenderer
		{
			get 
			{
				if (_imageRenderer == null)
					_imageRenderer = new VolumePresentationImageRenderer();

				return _imageRenderer;
			}
		}
	}
}
