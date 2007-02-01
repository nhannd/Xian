using System;
using System.Collections.Generic;
using System.Text;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume
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
