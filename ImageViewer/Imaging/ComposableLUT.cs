using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A LUT that can be added to <see cref="LutCollection"/>.
	/// </summary>
	public class ComposableLut : Lut, IComposableLut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		private event EventHandler _lutChangedEvent;

		protected ComposableLut()
		{

		}

		/// <summary>
		/// Initializes a new instance of a <see cref="ComposableLut"/> with an
		/// input range specified by the minimum and maximum input values.
		/// </summary>
		/// <param name="minInputValue"></param>
		/// <param name="maxInputValue"></param>
		public ComposableLut(
			int minInputValue,
			int maxInputValue) : base(maxInputValue - minInputValue + 1)
		{
			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;
		}

		#region IComposableLUT Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		public int MinInputValue
		{
			get { return _minInputValue; }
			protected set { _minInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		public int MaxInputValue
		{
			get { return _maxInputValue; }
			protected set { _maxInputValue = value; }
		}

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		public virtual int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		public virtual int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
		}

		/// <summary>
		/// Occurs when the LUT has changed.
		/// </summary>
		public event EventHandler LutChanged
		{
			add { _lutChangedEvent += value; }
			remove { _lutChangedEvent -= value; }
		}

		public virtual string GetKey()
		{
			return null;
		}

		#endregion

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				return base.Table[index - _minInputValue];
			}
			set
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				base.Table[index - _minInputValue] = value;
			}
		}

		/// <summary>
		/// Notify listeners that the LUT has changed.
		/// </summary>
		public void NotifyLutChanged()
		{
			EventsHelper.Fire(_lutChangedEvent, this, EventArgs.Empty);
		}
	}
}
