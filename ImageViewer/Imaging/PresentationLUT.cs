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
	public abstract class PresentationLut : DataLut, IPresentationLut
	{
		private bool _invert = false;
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
			: base(minInputValue, maxInputValue, int.MinValue, int.MaxValue)
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
			set
			{
				if (_invert == value)
					return;

				_invert = value;
				OnLutChanged();
			}
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
				if (_invert)
					return base[_minPlusMax - index];
				else
					return base[index];
			}
		}

		public sealed override string GetKey()
		{
			return GetKey(this.MinInputValue, this.MaxInputValue, this.Invert, this.GetType());
		}

		internal static string GetKey<T>(int minInputValue, int maxInputValue, bool invert)
			where T : PresentationLut
		{
			return GetKey(minInputValue, maxInputValue,	invert, typeof(T));
		}

		private static string GetKey(int minInputValue, int maxInputValue, bool invert, Type type)
		{
			return String.Format("{0}-{1}-{2}-{3}",
				minInputValue,
				maxInputValue,
				invert.ToString(),
				type.ToString());
		}

	}
}
