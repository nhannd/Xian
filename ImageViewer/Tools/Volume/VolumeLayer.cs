using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Layers;
using vtk;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public enum RenderingMethod
	{
		Surface,
		Volume
	}

	public class VolumeLayer : Layer
	{
		private TissueSettings _tissueSettings;
		private IVtkProp _surfaceProp;
		private IVtkProp _volumeProp;
		private RenderingMethod _renderingMethod = RenderingMethod.Surface;
	
		public VolumeLayer(TissueSettings tissueSettings) : base(true)
		{
			_tissueSettings = tissueSettings;
			_tissueSettings.VolumeLayer = this;
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

		protected override BaseLayerCollection CreateChildLayers()
		{
			return null;
		}
	}
}
