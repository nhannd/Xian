#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public interface ILayerOpacityManager : IMemorable
	{
		bool Enabled { get; set; }
		float Opacity { get; set; }
		bool Thresholding { get; set; }
	}

	public interface ILayerOpacityProvider : IDrawable
	{
		ILayerOpacityManager LayerOpacityManager { get; }
	}
}