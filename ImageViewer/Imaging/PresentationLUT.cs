using System;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Presentation LUT.
	/// </summary>
	/// <remarks>
	/// The <see cref="PresentationLut"/> is always the last LUT in the
	/// <see cref="LutCollection"/>.  The values in the LUT represent
	/// ARGB values that are used by the <see cref="IRenderer"/>
	/// to display the image.
	/// </remarks>
	public abstract class PresentationLut : GeneratedDataLut, IPresentationLut
	{
		private int _minPlusMax;
		private bool _invert = false;

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationLut"/>
		/// with the specified mininum/maximum input values and
		/// photometric interpretation.
		/// </summary>
		protected PresentationLut()
		{
			_invert = false;
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public sealed override int MinOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. ");
			}
			protected set
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. ");
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public sealed override int MaxOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a maximum output value. ");
			}
			protected set
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
		public sealed override int this[int index]
		{
			get
			{
				if (_invert)
					return base[_minPlusMax - index];
				else
					return base[index];
			}
			protected  set
			{
				base[index] = value;
			}
		}

		protected sealed override void OnLutChanged()
		{
			base.OnLutChanged();
			_minPlusMax = this.MinInputValue + this.MaxInputValue;
		}

		public sealed override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3}",
				this.MinInputValue,
				this.MaxInputValue,
				this.Invert.ToString(),
				this.GetType().ToString());
		}

		public sealed override IMemento CreateMemento()
		{
			return base.CreateMemento();
		}

		public sealed override void SetMemento(IMemento memento)
		{
			base.SetMemento(memento);
		}

		#region IEquatable<IPresentationLut> Members

		public bool Equals(IPresentationLut other)
		{
			return this.MinInputValue == other.MinInputValue && this.MaxInputValue == other.MaxInputValue &&
				this.Invert == other.Invert && this.GetType() == other.GetType();
		}

		#endregion
	}
}
