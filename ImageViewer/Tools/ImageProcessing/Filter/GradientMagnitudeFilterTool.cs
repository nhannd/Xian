#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.VtkItkAdapters;
using itk;
using FilterType = itk.itkGradientMagnitudeImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuGradientMagnitude", "Apply")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuGradientMagnitude", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class GradientMagnitudeFilterTool : ImageViewerTool
	{
        public GradientMagnitudeFilterTool()
		{

		}

		public void Apply()
		{
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

            itkImageBase input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            itkImageBase output = itkImage.New(input);
            ItkHelper.CopyToItkImage(image as GrayscaleImageGraphic, input);
            
            FilterType filter = FilterType.New(input, output);
            filter.SetInput(input);

            String mangledType = input.MangledTypeString;
            intensityFilterType intensityFilter = intensityFilterType.New(mangledType + mangledType);
            intensityFilter.SetInput(filter.GetOutput());
            intensityFilter.OutputMinimum = 0;
            if (image.BitsPerPixel == 16)
                intensityFilter.OutputMaximum = 32767;
            else
                intensityFilter.OutputMaximum = 255;
            intensityFilter.Update();

            intensityFilter.GetOutput(output);
            ItkHelper.CopyFromItkImage(image as GrayscaleImageGraphic, output);
            image.Draw();

            filter.Dispose();
            intensityFilter.Dispose();
            input.Dispose();
            output.Dispose();
		}
	}
}
