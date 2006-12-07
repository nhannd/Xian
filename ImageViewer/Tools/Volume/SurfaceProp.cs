using System;
using System.Collections.Generic;
using System.Text;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	class SurfaceProp : IVtkProp
	{
		private VolumeLayer _volumeLayer;
		private vtkActor _vtkActor;
		private vtkContourFilter _contourFilter;

		public SurfaceProp(VolumeLayer volumeLayer)
		{
			_volumeLayer = volumeLayer;
		}

		public vtkProp VtkProp
		{
			get
			{
				if (_vtkActor == null)
					CreateSurfaceRendering();

				return _vtkActor;
			}
		}

		private void CreateSurfaceRendering()
		{
			_contourFilter = new vtk.vtkContourFilter();
			_contourFilter.SetInput(_volumeLayer.GetImageData());
			_contourFilter.SetValue(0, _volumeLayer.GetRescaledLevel());

			vtkPolyDataNormals normals = new vtk.vtkPolyDataNormals();
			normals.SetInputConnection(_contourFilter.GetOutputPort());
			normals.SetFeatureAngle(60.0);

			vtkStripper stripper = new vtk.vtkStripper();
			stripper.SetInputConnection(normals.GetOutputPort());

			vtkPolyDataMapper mapper = new vtk.vtkPolyDataMapper();
			mapper.SetInputConnection(stripper.GetOutputPort());
			mapper.ScalarVisibilityOff();

			_vtkActor = new vtk.vtkActor();
			_vtkActor.SetMapper(mapper);
			_vtkActor.GetProperty().SetSpecular(.3);
			_vtkActor.GetProperty().SetSpecularPower(20);
			ApplySetting("Opacity");
			ApplySetting("Level");
		}

		public void ApplySetting(string setting)
		{
			if (setting == "Visible")
			{
				if (_volumeLayer.TissueSettings.Visible)
					_vtkActor.VisibilityOn();
				else
					_vtkActor.VisibilityOff();

				_vtkActor.ApplyProperties();
			}
			else if (setting == "Opacity")
			{
				_vtkActor.GetProperty().SetOpacity((double)_volumeLayer.TissueSettings.Opacity);
				_vtkActor.ApplyProperties();
			}
			else if (setting == "Level")
			{
				_contourFilter.SetValue(0, _volumeLayer.GetRescaledLevel());
				double R = _volumeLayer.TissueSettings.MinimumColor.R / 255.0f;
				double G = _volumeLayer.TissueSettings.MinimumColor.G / 255.0f;
				double B = _volumeLayer.TissueSettings.MinimumColor.B / 255.0f;
				_vtkActor.GetProperty().SetDiffuseColor(R, G, B);
			}
		}
	}
}
