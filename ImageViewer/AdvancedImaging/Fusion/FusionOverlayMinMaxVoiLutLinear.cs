#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	/// <summary>
	/// Delay-computed min/max calculated VOI LUT for the fusion data of a <see cref="FusionPresentationImage"/>.
	/// </summary>
	internal class FusionOverlayMinMaxVoiLutLinear : CalculatedVoiLutLinear
	{
		private readonly FusionPresentationImage _fusionPresentationImage;
		private readonly bool _useModalityLut;

		private double _windowWidth = double.NaN;
		private double _windowCenter = double.NaN;

		public FusionOverlayMinMaxVoiLutLinear(FusionPresentationImage fusionPresentationImage, bool useModalityLut)
		{
			Platform.CheckForNullReference(fusionPresentationImage, "fusionPresentationImage");
			_fusionPresentationImage = fusionPresentationImage;
			_useModalityLut = useModalityLut;
		}

		private void Calculate()
		{
			try
			{
				double windowStart = _fusionPresentationImage.OverlayFrameData.OverlayData.MinVolumeValue;
				double windowEnd = _fusionPresentationImage.OverlayFrameData.OverlayData.MaxVolumeValue;

				if (_useModalityLut)
				{
					var modalityLut = ((GrayscaleImageGraphic) _fusionPresentationImage.OverlayImageGraphic).ModalityLut;
					if (modalityLut != null)
					{
						windowStart = modalityLut[windowStart];
						windowEnd = modalityLut[windowEnd];
					}
				}

				// round the window to one decimal place so it's not ridiculous
				// value is calculated anyway and thus has no significance outside of display
				var windowWidth = Math.Max(windowEnd - windowStart + 1, 1);
				_windowWidth = Math.Round(windowWidth, 1);
				_windowCenter = Math.Round(windowStart + windowWidth/2, 1);
			}
			catch (Exception)
			{
				_windowWidth = _windowCenter = double.NaN;
			}
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format(@"Fusion Min/Max (W={0} C={1})", WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public override sealed double WindowWidth
		{
			get
			{
				if (double.IsNaN(_windowWidth))
					Calculate();

				return _windowWidth;
			}
		}

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public override sealed double WindowCenter
		{
			get
			{
				if (double.IsNaN(_windowCenter))
					Calculate();

				return _windowCenter;
			}
		}
	}
}