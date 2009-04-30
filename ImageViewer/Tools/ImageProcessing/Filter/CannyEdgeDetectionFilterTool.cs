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
using FilterType = itk.itkCannyEdgeDetectionImageFilter;
using CastImageFilterType = itk.itkCastImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
    [MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuCannyEdgeDetection", "Apply")]
    [MenuAction("apply", "imageviewer-filterdropdownmenu/MenuCannyEdgeDetection", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CannyEdgeDetectionFilterTool : ImageViewerTool
	{
        public CannyEdgeDetectionFilterTool()
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

            byte[] pixels = image.PixelData.Raw;

            itkImageBase input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            itkImageRegion region = input.LargestPossibleRegion;

            itkImageBase output = itkImage.New(input);
            ItkHelper.CopyToItkImage(image as GrayscaleImageGraphic, input);

            String mangledType = input.MangledTypeString;
            CastImageFilterType castToIF2 = CastImageFilterType.New(mangledType + "IF2");
            castToIF2.SetInput(input);

            FilterType filter = FilterType.New("IF2IF2");
            filter.SetInput(castToIF2.GetOutput());

            // TODO: need to allow user to set parameters of filter
            filter.LowerThreshold = 90;
            filter.UpperThreshold = 127;
            //filter.OutsideValue = 0;
            // smoothing the edge
            double[] error = {0.01, 0.01};
            filter.MaximumError = error;
            double[] var = { 1.0, 1.0 };
            filter.Variance = var;

            intensityFilterType intensityFilter = intensityFilterType.New("IF2" + mangledType);
            intensityFilter.SetInput(filter.GetOutput());
            intensityFilter.OutputMinimum = 0;
            if (image.BitsPerPixel == 16)
                intensityFilter.OutputMaximum = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;//32767;
            else
                intensityFilter.OutputMaximum = 255;
            intensityFilter.Update();

#if DEBUG
            bool debug = false;
            if (debug)
            {
                itkImageBase outputIF2 = itkImage.New("IF2");
                filter.GetOutput(outputIF2);
                float min = float.MaxValue, max = float.MinValue;
                unsafe
                {
                    fixed (byte* pDstByte = image.PixelData.Raw)
                    {
                        itkImageRegionConstIterator_IF2 itkIt = new itkImageRegionConstIterator_IF2(outputIF2, region);
                        byte* pDst = (byte*)pDstByte;
                        int height = image.Rows;
                        int width = image.Columns;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                float f = itkIt.Get().ValueAsF;
                                if (f > max)
                                    max = f;
                                if (f < min)
                                    min = f;
                                pDst[0] = (byte)itkIt.Get().ValueAsF;
                                pDst++;
                                itkIt++;
                            }
                        }
                    }
                }
                Console.WriteLine("min max "); Console.Write(min); Console.Write(" "); Console.WriteLine(max);
            }
#endif

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
