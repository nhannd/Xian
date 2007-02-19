using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class StandardPaletteColorImageGraphic : IndexedImageGraphic
	{
		private ImageSop _imageSop;

		public StandardPaletteColorImageGraphic(ImageSop imageSop) : base(imageSop)
		{
			_imageSop = imageSop;

			//TODO: Load palette tables into LUTs here
		}
	}
}
