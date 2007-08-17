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
		private byte[] _probabilityMap;
		private ColorImageGraphic _probabilityOverlay;

		#endregion

		public DynamicTePresentationImage(
			ImageSop imageSop, 
			byte[] protonDensityMap,
			byte[] t2Map,
			byte[] probabilityMap)
			: base(imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_dynamicTe = new DynamicTe(this.ImageGraphic as GrayscaleImageGraphic, protonDensityMap, t2Map);
			_probabilityMap = probabilityMap;

			AddProbabilityOverlay();
		}

		public DynamicTe DynamicTe
		{
			get { return _dynamicTe; }
		}

		protected override ImageGraphic CreateImageGraphic()
		{
			return new StandardGrayscaleImageGraphic(
				this.ImageSop,
				this.ImageSop.Rows,
				this.ImageSop.Columns);
		}

		public override IPresentationImage  Clone()
		{
 			 return new DynamicTePresentationImage(
				 this.ImageSop, 
				 this.DynamicTe.ProtonDensityMap, 
				 this.DynamicTe.T2Map,
				 _probabilityMap);
		}

		public void SetProbabilityThreshold(int probability, Color color)
		{
			IndexedPixelData probabilityPixelData = new IndexedPixelData(
				this.ImageSop.Rows,
				this.ImageSop.Columns,
				this.ImageSop.BitsAllocated,
				this.ImageSop.BitsStored,
				this.ImageSop.HighBit,
				this.ImageSop.PixelRepresentation != 0 ? true : false,
				_probabilityMap);

			probabilityPixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
				{
					if (probabilityPixelData.GetPixel(pixelIndex) < probability)
						_probabilityOverlay.PixelData.SetPixel(pixelIndex, color);
				});
		}

		private void AddProbabilityOverlay()
		{
			_probabilityOverlay = new ColorImageGraphic(this.ImageSop.Rows, this.ImageSop.Columns);
			this.OverlayGraphics.Add(_probabilityOverlay);
		}
	}
}
