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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public enum RenderingMethod
	{
		Surface,
		Volume
	}

	public class VolumeGraphic : Graphic
	{
		private TissueSettings _tissueSettings;
		private IVtkProp _surfaceProp;
		private IVtkProp _volumeProp;
		private RenderingMethod _renderingMethod = RenderingMethod.Surface;
	
		public VolumeGraphic(TissueSettings tissueSettings)
		{
			_tissueSettings = tissueSettings;
			_tissueSettings.VolumeGraphic = this;
			_tissueSettings.PropertyChanged += new PropertyChangedEventHandler(OnTissueSettingsChanged);
		}

		public RenderingMethod RenderingMethod
		{
			get { return _renderingMethod;}
			set 
			{
				if (_renderingMethod != value)
				{
					_renderingMethod = value;

					if (_renderingMethod == RenderingMethod.Surface)
					{
						this.SurfaceProp.VtkProp.VisibilityOn();
						this.SurfaceProp.ApplySetting("Opacity");
						this.SurfaceProp.ApplySetting("Level");

						this.VolumeProp.VtkProp.VisibilityOff();
					}
					else
					{
						this.VolumeProp.VtkProp.VisibilityOn();
						this.VolumeProp.ApplySetting("Opacity");
						this.VolumeProp.ApplySetting("Level");
						this.VolumeProp.ApplySetting("Window");

						this.SurfaceProp.VtkProp.VisibilityOff();
					}
				}
			}
		}

		public TissueSettings TissueSettings
		{
			get { return _tissueSettings; }
		}

		public vtkProp VtkProp
		{
			get 
			{
				if (this.RenderingMethod == RenderingMethod.Surface)
					return this.SurfaceProp.VtkProp;
				else
					return this.VolumeProp.VtkProp;
			}
		}

		private IVtkProp SurfaceProp
		{
			get
			{
				if (_surfaceProp == null)
					_surfaceProp = new SurfaceProp(this);

				return _surfaceProp;
			}
		}

		private IVtkProp VolumeProp
		{
			get
			{
				if (_volumeProp == null)
					_volumeProp = new VolumeProp(this);

				return _volumeProp;
			}
		}

		private VolumePresentationImage ParentVolumePresentationImage
		{
			get { return this.ParentPresentationImage as VolumePresentationImage; }
		}

		internal vtkImageData GetImageData()
		{
			vtkImageData imageData = this.ParentVolumePresentationImage.VtkImageData;
			return imageData;
		}
		
		internal double GetWindowLeft()
		{
			return GetRescaledLevel() - (double)_tissueSettings.Window / 2;
		}

		internal double GetWindowRight()
		{
			return GetRescaledLevel() +	(double)_tissueSettings.Window / 2;
		}

		internal double GetRescaledLevel()
		{
			return (double)_tissueSettings.Level -
				this.ParentVolumePresentationImage.RescaleIntercept -
				(double)this.ParentVolumePresentationImage.MinimumPixelValue;
		}

		void OnTissueSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SurfaceRenderingSelected")
			{
				if (_tissueSettings.SurfaceRenderingSelected)
					this.RenderingMethod = RenderingMethod.Surface;
			}
			else if (e.PropertyName == "VolumeRenderingSelected")
			{
				if (_tissueSettings.VolumeRenderingSelected)
					this.RenderingMethod = RenderingMethod.Volume;
			}
			else
			{
				if (_tissueSettings.SurfaceRenderingSelected)
					_surfaceProp.ApplySetting(e.PropertyName);
				else
					_volumeProp.ApplySetting(e.PropertyName);
			}
		}

		public override bool HitTest(Point point)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Move(SizeF delta)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
