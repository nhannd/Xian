using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

/*
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
			IStatefulVoiLutLinear statefulLinearLut = this.VoiLut as IStatefulVoiLutLinear;
			if (statefulLinearLut != null)
			{
				StandardGrayscaleImageGraphicVoiLutLinearState state = statefulLinearLut.State as StandardGrayscaleImageGraphicVoiLutLinearState;
				if (state != null)
				{
					++state.HeaderLutIndex;
					return;
				}
			}

			this.InstallVoiLut(this.CreateStatefulLut(new StandardGrayscaleImageGraphicVoiLutLinearState(0, _parentImageGraphic)));
		}

		#endregion

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

			//A slight hack.  Because some Luts and/or their mementos/states will need access to the image graphic itself,
			//and because we often use a memento from another image to change the lut state, we swap the graphic member temporarily
			//until we have successfully altered/installed the lut, then we swap it back.
			IStandardGrayscaleImageGraphicMemorableComposableLutMemento graphicMemento = lutMemento as IStandardGrayscaleImageGraphicMemorableComposableLutMemento;
			StandardGrayscaleImageGraphic oldGraphic = null;
			if (graphicMemento != null)
			{
				oldGraphic = graphicMemento.Graphic;
				graphicMemento.Graphic = _parentImageGraphic;
			}

			try
			{
				if (!this.VoiLut.TrySetMemento(lutMemento))
					this.InstallVoiLut(lutMemento.RestoreLut(_parentImageGraphic.ModalityLUT.MinOutputValue, _parentImageGraphic.ModalityLUT.MaxOutputValue));
			}
			finally
			{
				if (graphicMemento != null)
					graphicMemento.Graphic = oldGraphic;
			}
		}

		#endregion

		#region IVoiLutLinearFactory Members

		public IVOILUTLinear CreateLut()
		{
			return StandardGrayscaleImageGraphic.NewVoiLutLinear(_parentImageGraphic);
		}

		public IStatefulVoiLutLinear CreateStatefulLut()
		{
			return StandardGrayscaleImageGraphic.NewVoiLutLinear(_parentImageGraphic);
		}

		public IStatefulVoiLutLinear CreateStatefulLut(IVoiLutLinearState initialState)
		{
			return StandardGrayscaleImageGraphic.NewVoiLutLinear(_parentImageGraphic, initialState);
		}

		#endregion
	}
}
*/