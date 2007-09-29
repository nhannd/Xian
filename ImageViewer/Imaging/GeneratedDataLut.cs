using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base implementation for Data Luts that are purely generated, 
	/// usually based on an equation or algorithm.
	/// </summary>
	/// <remarks>
	/// Often, Linear Luts are created by deriving from this class to improve performance so that
	/// the calculation is only performed once.  For an example, see <see cref="ModalityLutLinear"/>.
	/// </remarks>
	public abstract class GeneratedDataLut : ComposableLut, IGeneratedDataLut
	{
		private int _minimumInputValue;
		private int _maximimInputValue;
		private int _minimumOutputValue;
		private int _maximumOutputValue;

		private int[] _data;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GeneratedDataLut()
		{
			_minimumInputValue = int.MinValue;
			_maximimInputValue = int.MaxValue;

			_minimumOutputValue = int.MinValue;
			_maximumOutputValue = int.MaxValue;
		}

		protected bool Created
		{
			get { return _data != null; }
		}

		/// <summary>
		/// Gets the Lut data, lazily created.
		/// </summary>
		protected int[] Data
		{ 
			get
			{
				if (_minimumInputValue == int.MinValue || _maximimInputValue == int.MaxValue)
					throw new InvalidOperationException(SR.ExceptionMinMaxInputValuesNotSet);

				if (_data == null)
					_data = new int[this.Length];

				return _data;
			}
		}

		/// <summary>
		/// Looks up and returns a value at a particular index in the Lut.
		/// </summary>
		/// <param name="index">the index</param>
		/// <returns>the value at the given index</returns>
		public override int this[int index]
		{
			get
			{
				if (_data == null)
				{
					Create();
					OnLutChanged();
				}

				if (index <= _minimumInputValue)
					return _data[0];
				else if (index >= this.MaxInputValue)
					return _data[this.Length - 1];
				else
					return _data[index - this.MinInputValue];
			}
			protected set
			{
				if (index < this.MinInputValue || index > this.MaxInputValue)
					return;

				this.Data[index - this.MinInputValue] = value;
			}
		}

		#region IDataLut Members

		/// <summary>
		/// Inheritors must implement this method and create the Lut using their particular algorithm.
		/// </summary>
		public abstract void Create();

		/// <summary>
		/// Clears the data in the Lut.  The Lut can be recreated at will by calling <see cref="Create"/>.
		/// </summary>
		public void Clear()
		{
			_data = null;
		}

		#endregion

		/// <summary>
		/// Returns the length of the Lut.
		/// </summary>
		public uint Length
		{
			get { return (uint)(this.MaxInputValue - this.MinInputValue + 1); }
		}

		/// <summary>
		/// Gets or sets the minimum input value.  This value will be set internally by the framework.
		/// </summary>
		public sealed override int MinInputValue
		{
			get { return _minimumInputValue; }
			set
			{
				if (_minimumInputValue == value)
					return;

				_minimumInputValue = value;
				Clear();
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.  This value will be set internally by the framework.
		/// </summary>
		public sealed override int MaxInputValue
		{
			get { return _maximimInputValue; }
			set
			{
				if (_maximimInputValue == value)
					return;

				_maximimInputValue = value;
				Clear();
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _minimumOutputValue; }
			protected set
			{
				if (_minimumOutputValue == value)
					return;
				
				_minimumOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _maximumOutputValue; }
			protected set
			{
				if (_maximumOutputValue == value)
					return;

				_maximumOutputValue = value;
				OnLutChanged();
			}
		}
	}
}
