using System;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class DicomPresentationImage : PresentationImage
	{
		private ImageSop _imageSop;
		private LayerGroup _imageLayerGroup;
		private ImageLayer _imageLayer;
		private GraphicLayer _graphicLayer;
		
		public DicomPresentationImage(ImageSop imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			// For standard DICOM image, we have an image layer and an overlay layer
			_imageSop = imageSop;
			_imageLayerGroup = new LayerGroup();
			_imageLayer = new DicomImageLayer(imageSop);
			_graphicLayer = new GraphicLayer();

			_imageLayerGroup.Layers.Add(_imageLayer);
			_imageLayerGroup.Layers.Add(_graphicLayer);
			base.LayerManager.RootLayerGroup.Layers.Add(_imageLayerGroup);

			_imageLayerGroup.Selected = true;
			_imageLayer.Selected = true;
			_graphicLayer.Selected = true;

			_imageLayerGroup.SpatialTransform.PixelSpacingX = _imageSop.PixelSpacingX;
			_imageLayerGroup.SpatialTransform.PixelSpacingY = _imageSop.PixelSpacingY;
			_imageLayerGroup.SpatialTransform.SourceRectangle = new Rectangle(0, 0, _imageLayer.Columns, _imageLayer.Rows);
			_imageLayerGroup.SpatialTransform.Calculate();

			InstallDefaultLUTs();
		}

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		private void InstallDefaultLUTs()
		{
			// If the image is a colour image, then the grayscale pipeline will be null.
			if (_imageLayer.GrayscaleLUTPipeline == null)
				return;

			if (_imageLayer.GrayscaleLUTPipeline.ModalityLUT == null)
			{
				ModalityLUTLinear modalityLUT =
					new ModalityLUTLinear(
					_imageSop.BitsStored,
					_imageSop.PixelRepresentation,
					_imageSop.RescaleSlope,
					_imageSop.RescaleIntercept);

				_imageLayer.GrayscaleLUTPipeline.ModalityLUT = modalityLUT;
			}

			WindowLevelOperator.InstallVOILUTLinear(this);
		}
	}
}
