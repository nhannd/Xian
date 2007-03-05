using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class ImageSpatialTransformMemento : IMemento
	{
		private bool _scaleToFit;
		private IMemento _spatialTransformMemento;

		public ImageSpatialTransformMemento()
		{

		}

		public IMemento SpatialTransformMemento
		{
			get { return _spatialTransformMemento; }
			set { _spatialTransformMemento = value; }
		}

		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set { _scaleToFit = value; }
		}
	}
}
