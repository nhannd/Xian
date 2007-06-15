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
	public class DynamicTePresentationImage : StandardPresentationImage, IDynamicTeProvider
	{
		#region Private fields

		private DynamicTe _dynamicTe;

		#endregion

		public DynamicTePresentationImage(
			ImageSop imageSop, 
			byte[] protonDensityMap,
			byte[] t2Map)
			: base(imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_dynamicTe = new DynamicTe(this.ImageGraphic, protonDensityMap, t2Map);
		}

		public DynamicTe DynamicTe
		{
			get { return _dynamicTe; }
		}

		protected override ImageGraphic CreateImageGraphic()
		{
			return new GrayscaleImageGraphic(
				this.ImageSop.Rows,
				this.ImageSop.Columns,
				this.ImageSop.BitsAllocated,
				this.ImageSop.BitsStored,
				this.ImageSop.HighBit,
				this.ImageSop.SamplesPerPixel,
				this.ImageSop.PixelRepresentation,
				this.ImageSop.PlanarConfiguration,
				this.ImageSop.PhotometricInterpretation,
				this.ImageSop.RescaleSlope,
				this.ImageSop.RescaleIntercept,
				null);
		}

		public override IPresentationImage  Clone()
		{
 			 return new DynamicTePresentationImage(
				 this.ImageSop, 
				 this.DynamicTe.ProtonDensityMap, 
				 this.DynamicTe.T2Map);
		}
	}
}
