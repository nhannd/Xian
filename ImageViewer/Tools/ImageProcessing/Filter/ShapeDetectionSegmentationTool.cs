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
using CastImageFilterType = itk.itkCastImageFilter;
using SmoothingFilterType = itk.itkCurvatureAnisotropicDiffusionImageFilter;
using GradientMagnitudeFilterType = itk.itkGradientMagnitudeRecursiveGaussianImageFilter;
using SigmoidFilterType = itk.itkSigmoidImageFilter;
using FastMarchingFilterType = itk.itkFastMarchingImageFilter;
using ShapeDetectionLevelSetFilterType = itk.itkShapeDetectionLevelSetImageFilter;
using BinaryThresholdFilterType = itk.itkBinaryThresholdImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuShapeDetectionSegmentation", "Apply")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuShapeDetectionSegmentation", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShapeDetectionSegmenatationTool : ImageViewerTool
	{
        public ShapeDetectionSegmenatationTool()
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

            String mangledType = input.MangledTypeString;
            CastImageFilterType castToIF2 = CastImageFilterType.New(mangledType + "IF2");

            SmoothingFilterType smoothingFilter = SmoothingFilterType.New("IF2IF2");
            smoothingFilter.TimeStep = 0.125;
            smoothingFilter.NumberOfIterations = 5;
            smoothingFilter.ConductanceParameter = 9.0;

            GradientMagnitudeFilterType gradientMagnitudeFilter = GradientMagnitudeFilterType.New("IF2IF2");
            gradientMagnitudeFilter.Sigma = 1.0;

            SigmoidFilterType sigmoidFilter = SigmoidFilterType.New("IF2IF2");
            sigmoidFilter.OutputMinimum = 0.0;
            sigmoidFilter.OutputMaximum = 1.0;
            sigmoidFilter.Alpha = -0.5;//-0.3
            sigmoidFilter.Beta = 3.0;//2.0

            FastMarchingFilterType fastMarchingFilter = FastMarchingFilterType.New("IF2IF2");
            double initialDistance = 5.0;
            double seedValue = -initialDistance;
            int[] seedPosition = {256, 256};// user input
            itkIndex seedIndex = new itkIndex(seedPosition);
            itkLevelSetNode[] trialPoints = { new itkLevelSetNode(seedValue, seedIndex) };
            fastMarchingFilter.TrialPoints = trialPoints;
            fastMarchingFilter.SpeedConstant = 1.0;
            fastMarchingFilter.StoppingValue = 600;

            ShapeDetectionLevelSetFilterType shapeDetectionFilter = ShapeDetectionLevelSetFilterType.New("IF2IF2F");//IF2IF2
            double curvatureScaling = 0.05;
            double propagationScaling = 1.0;
            shapeDetectionFilter.CurvatureScaling = curvatureScaling;
            shapeDetectionFilter.PropagationScaling = propagationScaling;
            shapeDetectionFilter.MaximumRMSError = 0.02;
            shapeDetectionFilter.NumberOfIterations = 800;

            BinaryThresholdFilterType binaryThresholdFilter = BinaryThresholdFilterType.New("IF2" + mangledType);//to UC2?
            binaryThresholdFilter.UpperThreshold = 0.0; // to display the zero set of the resulting level set
            binaryThresholdFilter.LowerThreshold = -1000; // large negative 
            binaryThresholdFilter.OutsideValue = 0;
            if (image.BitsPerPixel == 16)
                binaryThresholdFilter.InsideValue = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;//32767;
            else
                binaryThresholdFilter.InsideValue = 255;

            //intensityFilterType intensityFilter = intensityFilterType.New("UC2" + mangledType);
            //intensityFilter.OutputMinimum = 0;
            //if (image.BitsPerPixel == 16)
            //    intensityFilter.OutputMaximum = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;//32767;
            //else
            //    intensityFilter.OutputMaximum = 255;

            // Make data stream connections
            castToIF2.SetInput(input);
            smoothingFilter.SetInput(castToIF2.GetOutput());
            gradientMagnitudeFilter.SetInput(smoothingFilter.GetOutput());
            sigmoidFilter.SetInput(gradientMagnitudeFilter.GetOutput());
            shapeDetectionFilter.SetInput(fastMarchingFilter.GetOutput());
            shapeDetectionFilter.SetFeatureImage(sigmoidFilter.GetOutput());
            binaryThresholdFilter.SetInput(shapeDetectionFilter.GetOutput());
            //intensityFilter.SetInput(binaryThresholdFilter.GetOutput());

            //smoothingFilter.Update();
            fastMarchingFilter.OutputSize = input.BufferedRegion.Size;//?
            binaryThresholdFilter.Update();

            binaryThresholdFilter.GetOutput(output);
            ItkHelper.CopyFromItkImage(image as GrayscaleImageGraphic, output);
            image.Draw();

            input.Dispose();
            output.Dispose();
		}
	}
}
