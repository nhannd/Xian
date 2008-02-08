#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	internal static class AutoPresetVoiLutOperationHelper
	{
		public static bool AppliesTo(IPresentationImage presentationImage)
		{
			if (!(presentationImage is IVoiLutProvider))
				return false;

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
			if (sopProvider != null && sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
				return true;

			IImageGraphicProvider graphicProvider = presentationImage as IImageGraphicProvider;
			if (graphicProvider != null)
				return graphicProvider.ImageGraphic.PixelData is GrayscalePixelData;

			return false;
		}

		public static void AutoApplyLut(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException(SR.ExceptionInputPresentationImageNotSupported);

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			IComposableLut currentLut = manager.GetLut();

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
			if (sopProvider != null)
			{
				if (sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
				{
					if (currentLut is AutoVoiLutLinear)
						((AutoVoiLutLinear)currentLut).ApplyNext();
					else
						manager.InstallLut(new AutoVoiLutLinear(sopProvider.ImageSop));

					return;
				}
			}

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			GrayscalePixelData pixelData = (GrayscalePixelData)((IImageGraphicProvider)presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut));
			else
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData));
		}

		public static IComposableLut GetInitialLut(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				return null;

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
			if (sopProvider != null)
			{
				if (sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
					return new AutoVoiLutLinear(sopProvider.ImageSop);
			}

			GrayscalePixelData pixelData = (GrayscalePixelData)((IImageGraphicProvider)presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				return new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut);
			else
				return new MinMaxPixelCalculatedLinearLut(pixelData);
		}
	}
}
