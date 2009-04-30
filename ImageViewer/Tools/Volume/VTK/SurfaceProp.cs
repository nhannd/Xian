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
	class SurfaceProp : IVtkProp
	{
		private VolumeGraphic _volumeGraphic;
		private vtkActor _vtkActor;
		private vtkContourFilter _contourFilter;

		public SurfaceProp(VolumeGraphic volumeLayer)
		{
			_volumeGraphic = volumeLayer;
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
			_contourFilter.SetInput(_volumeGraphic.GetImageData());
			_contourFilter.SetValue(0, _volumeGraphic.GetRescaledLevel());

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
				if (_volumeGraphic.TissueSettings.Visible)
					_vtkActor.VisibilityOn();
				else
					_vtkActor.VisibilityOff();

				_vtkActor.ApplyProperties();
			}
			else if (setting == "Opacity")
			{
				_vtkActor.GetProperty().SetOpacity((double)_volumeGraphic.TissueSettings.Opacity);
				_vtkActor.ApplyProperties();
			}
			else if (setting == "Level")
			{
				_contourFilter.SetValue(0, _volumeGraphic.GetRescaledLevel());
				double R = _volumeGraphic.TissueSettings.MinimumColor.R / 255.0f;
				double G = _volumeGraphic.TissueSettings.MinimumColor.G / 255.0f;
				double B = _volumeGraphic.TissueSettings.MinimumColor.B / 255.0f;
				_vtkActor.GetProperty().SetDiffuseColor(R, G, B);
			}
		}
	}
}
