using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTePresentationImage 
		: GrayscalePresentationImage, 
		IImageSopProvider, 
		IDynamicTeProvider
	{
		#region Private fields

		private ImageSop _imageSop;
		private DynamicTe _dynamicTe;
		private ColorImageGraphic _probabilityOverlay;

		#endregion

		public DynamicTePresentationImage(
			ImageSop imageSop, 
			byte[] protonDensityMap,
			byte[] t2Map,
			byte[] probabilityMap)
			: base(imageSop.Rows, imageSop.Columns)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_imageSop = imageSop;
			this.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			AddProbabilityOverlay();
			_dynamicTe = new DynamicTe(
				this.ImageGraphic as GrayscaleImageGraphic, 
				protonDensityMap, 
				t2Map,
				_probabilityOverlay,
				probabilityMap);
		}

		public DynamicTe DynamicTe
		{
			get { return _dynamicTe; }
		}

		public bool ProbabilityOverlayVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		#region IImageSopProvider Members

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		#endregion

		public override IPresentationImage  Clone()
		{
 			 return new DynamicTePresentationImage(
				 this.ImageSop, 
				 this.DynamicTe.ProtonDensityMap, 
				 this.DynamicTe.T2Map,
				 this.DynamicTe.ProbabilityMap);
		}


		private void AddProbabilityOverlay()
		{
			_probabilityOverlay = new ColorImageGraphic(this.ImageSop.Rows, this.ImageSop.Columns);
			this.OverlayGraphics.Add(_probabilityOverlay);
		}
	}
}
