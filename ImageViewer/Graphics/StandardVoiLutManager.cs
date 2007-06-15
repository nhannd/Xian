using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class StandardVoiLutManager : IVoiLutManager, IAutoVoiLutApplicator
	{
		private StandardGrayscaleImageGraphic _parentImageGraphic;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parentImageGraphic">the parent image graphic</param>
		public StandardVoiLutManager(StandardGrayscaleImageGraphic parentImageGraphic)
		{
			_parentImageGraphic = parentImageGraphic;
		}

		private StandardVoiLutManager()
		{
		}

		/// <summary>	
		/// Gets the Window Width/Center values from the Dicom Header of the parent image.
		/// </summary>
		protected Window[] LinearHeaderLuts
		{
			get { return _parentImageGraphic.ImageSop.WindowCenterAndWidth; }
		}

		/// <summary>
		/// Returns the number of Linear Header Luts in the Dicom Header of the parent image.
		/// </summary>
		protected int NumberOfLinearHeaderLuts
		{
			get { return this.AnyLinearHeaderLuts ? this.LinearHeaderLuts.Length : 0; }
		}

		/// <summary>
		/// Returns whether or not there are any Linear Header Luts in the Dicom Header of the parent image.
		/// </summary>
		protected bool AnyLinearHeaderLuts
		{
			get { return !(this.LinearHeaderLuts == null || this.LinearHeaderLuts.Length == 0); }
		}

		protected IMemorableComposableLut VoiLut
		{
			get { return _parentImageGraphic.VoiLUT as IMemorableComposableLut; }
		}

		#region IVoiLutManager Members

		public virtual void InstallVoiLut(IComposableLUT newLut)
		{
			//enforce that any luts installed are memorable composable luts, for this Lut Manager, at least.
			Platform.CheckForNullReference(newLut, "newLut");
			IMemorableComposableLut memorableLut = newLut as IMemorableComposableLut;
			Platform.CheckForInvalidCast(memorableLut, "newLut", "IMemorableComposableLut");

			_parentImageGraphic.InstallVoiLut(newLut);
		}

		#endregion

		#region IAutoVoiLutApplicator Members

		/// <summary>
		/// Applies the next Lut from the header.
		/// </summary>
		/// <remarks>
		/// This method, when called repeatedly, will apply the header Luts in a cyclic 
		/// fashion (0, 1, 2, 0, 1, 2 ...).  If a header lut is not currently applied, 
		/// the first Lut from the header is applied.  If there are no header Luts, 
		/// the minimum and maximum pixel values from the pixel data are used to calculate the w/l.
		/// </remarks>
		public virtual void ApplyNext()
		{
			//Right now, this VoiLutManager only understands VoiLutLinear 'auto' luts.  In the
			//future, it may also be able to apply data luts from the header.
			VOILUTLinear linearLut = this.VoiLut as VOILUTLinear;
			if (linearLut != null)
			{
				HeaderVoiLutLinearState state = linearLut.State as HeaderVoiLutLinearState;
				if (state != null)
				{
					int newIndex = state.HeaderLutIndex + 1;
					if (newIndex >= this.NumberOfLinearHeaderLuts)
						newIndex = 0;

					if (state.HeaderLutIndex == newIndex)
						return;

					linearLut.State = new HeaderVoiLutLinearState(newIndex, GetHeaderWindowLevelValues);
					return;
				}
			}

			this.InstallVoiLut(GrayscaleImageGraphic.NewVoiLutLinear(_parentImageGraphic, new HeaderVoiLutLinearState(0, this.GetHeaderWindowLevelValues)));
		}

		#endregion

		/// <summary>
		/// Delegate passed to a <see cref="HeaderVoiLutLinearState"/> to get the Window/Level values from the image header on demand.
		/// If no Luts exist in the header, the minimum and maximum pixel values are calculated from the image pixel data.
		/// </summary>
		/// <param name="headerLutIndex">the index of the header lut</param>
		/// <param name="windowWidth">returns the window width</param>
		/// <param name="windowCenter">returns the window center</param>
		protected void GetHeaderWindowLevelValues(int headerLutIndex, out double windowWidth, out double windowCenter)
		{
			Platform.CheckArgumentRange(headerLutIndex, 0, this.NumberOfLinearHeaderLuts, "headerLutIndex");

			if (!this.AnyLinearHeaderLuts)
			{
				//just computing the min/max pixel value for now.  Could later use an algorithm, possibly via an extension point.
				int minPixelValue = _parentImageGraphic.ModalityLUT[_parentImageGraphic.MinPixelValue];
				int maxPixelValue = _parentImageGraphic.ModalityLUT[_parentImageGraphic.MaxPixelValue];

				windowWidth = (maxPixelValue - minPixelValue) + 1;
				windowCenter = Math.Truncate(minPixelValue + windowWidth / 2.0);
			}
			else
			{
				windowWidth = this.LinearHeaderLuts[headerLutIndex].Width;
				windowCenter = this.LinearHeaderLuts[headerLutIndex].Center;
			}
		}

		#region IMemorable Members

		/// <summary>
		/// Implements the Memento pattern.  This method creates a memento for the Voi
		/// Lut that is currently applied so that it can be restored by an undo operation.
		/// </summary>
		/// <returns>an <see cref="IMemento"/> that is capable of restoring the previous lut state.</returns>
		public IMemento CreateMemento()
		{
			return this.VoiLut.CreateMemento();
		}

		/// <summary>
		/// Implements the memento pattern.  This method changes the parent image's Voi Lut based on the
		/// supplied memento.
		/// </summary>
		/// <param name="memento">the memento to use to change the parent image's Voi Lut.</param>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			IMemorableComposableLutMemento lutMemento = memento as IMemorableComposableLutMemento;
			Platform.CheckForInvalidCast(lutMemento, "memento", "IMemorableComposableLutMemento");

			if (!this.VoiLut.TrySetMemento(lutMemento))
				this.InstallVoiLut(lutMemento.RestoreLut());
		}

		#endregion
	}
}
