using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Layers;
using vtk;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumeLayer : Layer
	{
		private TissueSettings _tissueSettings;
		private vtkProp _vtkProp;
		private vtkPiecewiseFunction _opacityTransferFunction;
		private vtkColorTransferFunction _colorTransferFunction;
		
		public VolumeLayer(TissueSettings tissueSettings) : base(true)
		{
			_tissueSettings = tissueSettings;
			_tissueSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnTissueSettingsChanged);
		}

		public TissueSettings TissueSettings
		{
			get { return _tissueSettings; }
		}

		public vtkProp VtkProp
		{
			get 
			{
				if (_vtkProp == null)
					//_vtkProp = CreateIsocontourVolume(GetImageData());
					_vtkProp = CreateVolumeRendering(GetImageData());

				return _vtkProp;
			}
		}

		private VolumePresentationImage ParentVolumePresentationImage
		{
			get { return this.ParentPresentationImage as VolumePresentationImage; }
		}

		private vtkImageData GetImageData()
		{
			vtkImageData imageData = this.ParentVolumePresentationImage.VtkImageData;
			return imageData;
		}
		
		private vtkProp CreateIsocontourVolume(vtkImageData imageData)
		{
			vtkContourFilter extractor = new vtk.vtkContourFilter();
			extractor.SetInput(imageData);
			//extractor.SetValue(0, 100);
			extractor.SetValue(0, 400);
			//extractor.GenerateValues(5, 2, 30);

			vtkPolyDataNormals normals = new vtk.vtkPolyDataNormals();
			normals.SetInputConnection(extractor.GetOutputPort());
			normals.SetFeatureAngle(60.0);

			vtkStripper stripper = new vtk.vtkStripper();
			stripper.SetInputConnection(normals.GetOutputPort());

			vtkPolyDataMapper mapper = new vtk.vtkPolyDataMapper();
			mapper.SetInputConnection(stripper.GetOutputPort());
			mapper.ScalarVisibilityOff();

			vtkActor actor = new vtk.vtkActor();
			actor.SetMapper(mapper);
			actor.GetProperty().SetDiffuseColor(1, 1, .9412);
			//skin.GetProperty().SetDiffuseColor(1, .49, .25);
			actor.GetProperty().SetSpecular(.3);
			actor.GetProperty().SetSpecularPower(20);

			return actor;
		}

		private vtkProp CreateVolumeRendering(vtkImageData imageData)
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

			vtkOpenGLVolumeTextureMapper3D volumeMapper = new vtkOpenGLVolumeTextureMapper3D();
			volumeMapper.SetPreferredMethodToNVidia();
			volumeMapper.SetInput(imageData);
			int supported = volumeMapper.IsRenderSupported(volumeProperty);

			//vtkFixedPointVolumeRayCastMapper volumeMapper = new vtkFixedPointVolumeRayCastMapper();
			//volumeMapper.SetInput(imageData);

			vtkVolume volume = new vtkVolume();
			volume.SetMapper(volumeMapper);
			volume.SetProperty(volumeProperty);

			return volume;
		}

		private void SetOpacityTransferFunction()
		{
			_opacityTransferFunction.RemoveAllPoints();
			_opacityTransferFunction.AddPoint(GetWindowLeft(), 0.0);
			_opacityTransferFunction.AddPoint(GetRescaledLevel(), (double)_tissueSettings.Opacity);
			_opacityTransferFunction.AddPoint(GetWindowRight(), 0.0);
		}

		private void SetColorTransferFunction()
		{
			_colorTransferFunction.RemoveAllPoints();

			double R = _tissueSettings.MinimumColor.R / 255.0f;
			double G = _tissueSettings.MinimumColor.G / 255.0f;
			double B = _tissueSettings.MinimumColor.B / 255.0f;

			_colorTransferFunction.AddRGBPoint(GetWindowLeft(), R, G, B);

			R = _tissueSettings.MaximumColor.R / 255.0f;
			G = _tissueSettings.MaximumColor.G / 255.0f;
			B = _tissueSettings.MaximumColor.B / 255.0f;

			_colorTransferFunction.AddRGBPoint(GetWindowRight(), R, G, B);
		}

		private double GetWindowLeft()
		{
			return GetRescaledLevel() - (double)_tissueSettings.Window / 2;
		}

		private double GetWindowRight()
		{
			return GetRescaledLevel() +	(double)_tissueSettings.Window / 2;
		}

		private double GetRescaledLevel()
		{
			return (double)_tissueSettings.Level -
				this.ParentVolumePresentationImage.RescaleIntercept;
		}

		void OnTissueSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			SetOpacityTransferFunction();

			if (e.PropertyName != "OpacityValue")
				SetColorTransferFunction();

			vtkVolume volume = vtkVolume.SafeDownCast(_vtkProp);
			volume.Update();
		}


		protected override BaseLayerCollection CreateChildLayers()
		{
			return null;
		}
	}
}
