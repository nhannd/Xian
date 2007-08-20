using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;

/*
namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// The base state for <see cref="StandardGrayscaleImageGraphic"/> objects.  Classes derived from this one have an opportunity
	/// to provide initial window width / center values the first time either the <see cref="WindowWidth"/> or <see cref="WindowCenter"/>
	/// properties' getter is accessed.  Any arbitrary algorithm can be used.
	/// </summary>
	public abstract class StandardGrayscaleImageGraphicVoiLutLinearStateBase : VoiLutLinearState, IStandardGrayscaleImageGraphicMemorableComposableLutMemento
	{
		private StandardGrayscaleImageGraphic _graphic;
		private double _windowWidth = double.NaN;
		private double _windowCenter = double.NaN;

		public StandardGrayscaleImageGraphicVoiLutLinearStateBase(StandardGrayscaleImageGraphic graphic)
		{
			_graphic = graphic;
		}

		protected StandardGrayscaleImageGraphicVoiLutLinearStateBase()
		{ 
		}

		#region IStandardGrayscaleImageGraphicMemorableComposableLutMemento Members

		/// <summary>
		/// The graphic that should be used for calculating the intial window width / center.
		/// </summary>
		public StandardGrayscaleImageGraphic Graphic
		{
			get
			{
				return _graphic;
			}
			set
			{
				_graphic = value;
			}
		}

		#endregion

		/// <summary>
		/// The window width value.  On first accessing the get method, both the window width and center are calculated.
		/// If set, the <see cref="VoiLutLinearState.OwnerLut"/>'s state transitions to <see cref="StandardVoiLutLinearState"/>.
		/// </summary>
		public override double WindowWidth
		{
			get
			{
				Calculate();
				return _windowWidth;
			}
			set
			{
				base.OwnerLut.State = new StandardVoiLutLinearState(value, _windowCenter);
			}
		}

		/// <summary>
		/// The window center value.  On first accessing the get method, both the window width and center are calculated.
		/// If set, the <see cref="OwnerLut"/>'s state transitions to <see cref="StandardVoiLutLinearState"/>.
		/// </summary>
		public override double WindowCenter
		{
			get
			{
				Calculate();
				return _windowCenter;
			}
			set
			{
				base.OwnerLut.State = new StandardVoiLutLinearState(_windowWidth, value);
			}
		}

		/// <summary>
		/// This method should be overridden to provide initial values for the <see cref="WindowWidth"/> and <see cref="WindowCenter"/>.
		/// </summary>
		/// <param name="windowWidth">returns the initial window width</param>
		/// <param name="windowCenter">returns the initial window center</param>
		protected abstract void Calculate(out double windowWidth, out double windowCenter);

		private void Calculate()
		{
			if (!double.IsNaN(_windowCenter) && !double.IsNaN(_windowWidth))
				return;

			Calculate(out _windowWidth, out _windowCenter);

			if (double.IsNaN(_windowCenter) || double.IsNaN(_windowWidth))
				throw new Exception("The calculated window width / center are invalid");
		}
	}
}
*/