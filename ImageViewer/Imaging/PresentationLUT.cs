using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Presentation LUT.
	/// </summary>
	/// <remarks>
	/// The <see cref="PresentationLUT"/> is always the last LUT in the
	/// <see cref="LUTCollection"/>.  The values in the LUT represent
	/// ARGB values that are used by the <see cref="IRenderer"/>
	/// to display the image.
	/// </remarks>
	public abstract class PresentationLut : ComposableLut, IPresentationLut
	{
		private bool _invert = false;
		private bool _lutCreated = false;
		private int _minPlusMax;

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationLUT"/>
		/// with the specified mininum/maximum input values and
		/// photometric interpretation.
		/// </summary>
		/// <param name="minInputValue"></param>
		/// <param name="maxInputValue"></param>
		/// <param name="invert"></param>
		protected PresentationLut(
			int minInputValue, 
			int maxInputValue,
			bool invert)
			: base(minInputValue, maxInputValue)
		{
			_invert = invert;
			_minPlusMax = this.MinInputValue + this.MaxInputValue;
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public override int MinOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. ");
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public override int MaxOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a maximum output value. ");
			}
		}

		/// <summary>
		/// Gets or sets a valud indicating whether the LUT is inverted.
		/// </summary>
		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public override int this[int index]
		{
			get
			{
				if (!_lutCreated)
				{
					CreateLut();
					_lutCreated = true;
				}

				if (_invert)
					return base[_minPlusMax - index];
				else
					return base[index];
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}-{1}-{2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetType().ToString());
		}

		protected abstract void CreateLut();

		#region IPresentationLut Members

		public abstract string Name { get; }

		#endregion
	}
}
