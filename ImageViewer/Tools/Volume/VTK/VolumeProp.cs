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
using System.Collections.Generic;
using System.Text;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	class VolumeProp : IVtkProp
	{
		private VolumeGraphic _volumeLayer;
		private vtkVolume _vtkVolume;
		private vtkPiecewiseFunction _opacityTransferFunction;
		private vtkColorTransferFunction _colorTransferFunction;

		public VolumeProp(VolumeGraphic volumeLayer)
		{
			_volumeLayer = volumeLayer;
		}

		public vtkProp VtkProp
		{
			get
			{
				if (_vtkVolume == null)
					CreateVolumeRendering();

				return _vtkVolume;
			}
		}

		private vtkProp CreateVolumeRendering()
		{
			_opacityTransferFunction = new vtkPiecewiseFunction();
			_opacityTransferFunction.ClampingOff();

			_colorTransferFunction = new vtkColorTransferFunction();
			_colorTransferFunction.SetColorSpaceToRGB();
			_colorTransferFunction.ClampingOff();

			SetOpacityTransferFunction();
			SetColorTransferFunction();

			vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
			volumeProperty.ShadeOn();
			volumeProperty.SetInterpolationTypeToLinear();
			volumeProperty.SetColor(_colorTransferFunction);
			volumeProperty.SetScalarOpacity(_opacityTransferFunction);
			volumeProperty.SetDiffuse(0.7);
			volumeProperty.SetAmbient(0.1);
			volumeProperty.SetSpecular(.3);
			volumeProperty.SetSpecularPower(20);

			//vtkOpenGLVolumeTextureMapper2D volumeMapper = new vtkOpenGLVolumeTextureMapper2D();
			//vtkOpenGLVolumeTextureMapper3D volumeMapper = new vtkOpenGLVolumeTextureMapper3D();
			//volumeMapper.SetPreferredMethodToNVidia();
			//volumeMapper.SetSampleDistance(1.0f);
			//int supported = volumeMapper.IsRenderSupported(volumeProperty);

			vtkFixedPointVolumeRayCastMapper volumeMapper = new vtkFixedPointVolumeRayCastMapper();
			//vtkVolumeRayCastMapper volumeMapper = new vtkVolumeRayCastMapper();
			volumeMapper.SetInput(_volumeLayer.GetImageData());
			////vtkVolumeRayCastCompositeFunction rayCastFunction = new vtkVolumeRayCastCompositeFunction();
			////volumeMapper.SetVolumeRayCastFunction(rayCastFunction);
			//vtkVolumeRayCastIsosurfaceFunction rayCastFunction = new vtkVolumeRayCastIsosurfaceFunction();
			//volumeMapper.SetVolumeRayCastFunction(rayCastFunction);

			_vtkVolume = new vtkVolume();
			_vtkVolume.SetMapper(volumeMapper);
			_vtkVolume.SetProperty(volumeProperty);

			return _vtkVolume;
		}

		private void SetOpacityTransferFunction()
		{
			_opacityTransferFunction.RemoveAllPoints();
			_opacityTransferFunction.AddPoint(_volumeLayer.GetWindowLeft(), 0.0);
			_opacityTransferFunction.AddPoint(
				_volumeLayer.GetRescaledLevel(), 
				(double)_volumeLayer.TissueSettings.Opacity);
			_opacityTransferFunction.AddPoint(_volumeLayer.GetWindowRight(), 0.0);
		}

		private void SetColorTransferFunction()
		{
			_colorTransferFunction.RemoveAllPoints();

			double R = _volumeLayer.TissueSettings.MinimumColor.R / 255.0f;
			double G = _volumeLayer.TissueSettings.MinimumColor.G / 255.0f;
			double B = _volumeLayer.TissueSettings.MinimumColor.B / 255.0f;

			_colorTransferFunction.AddRGBPoint(_volumeLayer.GetWindowLeft(), R, G, B);

			R = _volumeLayer.TissueSettings.MaximumColor.R / 255.0f;
			G = _volumeLayer.TissueSettings.MaximumColor.G / 255.0f;
			B = _volumeLayer.TissueSettings.MaximumColor.B / 255.0f;

			_colorTransferFunction.AddRGBPoint(_volumeLayer.GetWindowRight(), R, G, B);
		}

		public void ApplySetting(string setting)
		{
			if (setting == "Visible")
			{
				if (_volumeLayer.TissueSettings.Visible)
					_vtkVolume.VisibilityOn();
				else
					_vtkVolume.VisibilityOff();
			}
			else
			{
				SetOpacityTransferFunction();

				if (setting != "Opacity")
					SetColorTransferFunction();
			}
			_vtkVolume.Update();
		}


	}
}
