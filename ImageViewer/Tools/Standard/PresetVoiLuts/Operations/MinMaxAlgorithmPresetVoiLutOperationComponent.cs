#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public sealed class MinMaxAlgorithmPresetVoiLutOperationComponent : DefaultPresetVoiLutOperationComponent
	{
		public MinMaxAlgorithmPresetVoiLutOperationComponent()
		{
		}

		public override string Name
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutOperationComponentName; }
		}

		public override string Description
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutOperationComponentDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return base.AppliesTo(presentationImage) && LutHelper.IsGrayScaleImage(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException("The input presentation image is not supported.");

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			IVoiLut currentLut = manager.VoiLut;

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			GrayscalePixelData pixelData = (GrayscalePixelData)((IImageGraphicProvider) presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				manager.InstallVoiLut(new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut));
			else
				manager.InstallVoiLut(new MinMaxPixelCalculatedLinearLut(pixelData));
		}
	}
}