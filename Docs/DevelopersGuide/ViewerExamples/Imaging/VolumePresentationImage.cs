#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	public class VolumePresentationImage
		: PresentationImage, ISpatialTransformProvider, IVoiLutProvider
	{
		private IDisplaySet _displaySet;

		public VolumePresentationImage(IDisplaySet displaySet)
		{
			_displaySet = displaySet;
		}

		public override IRenderer ImageRenderer
		{
			get
			{
				// we may need to provide a custom renderer, such as this one
				// (in the ClearCanvas.ImageViewer.Tools.Volume.VTK project)
				if (base.ImageRenderer == null)
				{
					// base.ImageRenderer = new VolumePresentationImageRenderer();
				}

				return base.ImageRenderer;
			}
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new VolumePresentationImage(_displaySet);
		}

		public IVoiLutManager VoiLutManager
		{
			get
			{
				// IVoiLutProvider implementation
				return null;
			}
		}

		public ISpatialTransform SpatialTransform
		{
			get
			{
				// ISpatialTransformProvider implementation
				return null;
			}
		}
	}
}