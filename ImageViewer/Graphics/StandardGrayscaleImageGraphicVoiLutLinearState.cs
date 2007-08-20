using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Common;

/*
namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// The default state for <see cref="StandardGrayscaleImageGraphic"/> objects.  Currently, it will calculate the
	/// window center/width based on the values taken from the Dicom image header and if there are none, it will use the
	/// minimum and maximum pixel values calculated from the image pixel data.
	/// </summary>
	public sealed class StandardGrayscaleImageGraphicVoiLutLinearState : StandardGrayscaleImageGraphicVoiLutLinearStateBase
	{
		private int _headerLutIndex = 0;

		public StandardGrayscaleImageGraphicVoiLutLinearState(int headerLutIndex, StandardGrayscaleImageGraphic graphic)
			: base(graphic)
		{
			_headerLutIndex = headerLutIndex;
		}

		private StandardGrayscaleImageGraphicVoiLutLinearState()
		{ 
		}

		/// <summary>
		/// Gets/sets the index of the header Lut to apply.  The setter will wrap the index automatically if it exceeds the 
		/// number of Header Luts that exist.
		/// </summary>
		public int HeaderLutIndex
		{
			get { return _headerLutIndex; }
			set
			{
				//wraps the index around, so it can just be incremented by clients.
				if (value < 0 || value >= this.Graphic.NumberOfWindowCenterValues)
					value = 0;

				if (_headerLutIndex == value)
					return;

				StandardGrayscaleImageGraphicVoiLutLinearState newState = new StandardGrayscaleImageGraphicVoiLutLinearState(value, this.Graphic);
				base.OwnerLut.State = newState;
			}
		}

		protected override void Calculate(out double windowWidth, out double windowCenter)
		{
			if (!this.Graphic.AnyWindowCenterValues)
			{
				//just computing the min/max pixel value for now.  Could later use an algorithm via an extension point.
				int minPixelValue = this.Graphic.ModalityLUT[this.Graphic.MinPixelValue];
				int maxPixelValue = this.Graphic.ModalityLUT[this.Graphic.MaxPixelValue];

				windowWidth = (maxPixelValue - minPixelValue) + 1;
				windowCenter = Math.Truncate(minPixelValue + windowWidth / 2.0);
			}
			else
			{
				windowWidth = this.Graphic.WindowCenterValues[_headerLutIndex].Width;
				windowCenter = this.Graphic.WindowCenterValues[_headerLutIndex].Center;
			}
		}

		public override IMemorableComposableLutMemento SnapshotMemento()
		{
			return new StandardGrayscaleImageGraphicVoiLutLinearState(this.HeaderLutIndex, this.Graphic);
		}

		public override bool Equals(IVoiLutLinearState other)
		{
			if (other == null)
				return false;

			StandardGrayscaleImageGraphicVoiLutLinearState otherState = other as StandardGrayscaleImageGraphicVoiLutLinearState;
			if (otherState == null)
				return false;

			return (otherState.HeaderLutIndex == this.HeaderLutIndex);
		}
	}
}
*/