using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class GeneratedDataLut : Lut, IGeneratedDataLut
	{
		private int _minimumInputValue;
		private int _maximimInputValue;
		private int _minimumOutputValue;
		private int _maximumOutputValue;

		private int[] _data;

		protected GeneratedDataLut()
		{
			_minimumInputValue = int.MinValue;
			_maximimInputValue = int.MaxValue;

			_minimumOutputValue = int.MinValue;
			_maximumOutputValue = int.MaxValue;
		}

		protected int[] Data
		{ 
			get
			{
				if (_minimumInputValue == int.MinValue || _maximimInputValue == int.MaxValue)
					throw new InvalidOperationException("The minimum and maximum input values have not been set.");

				if (_data == null)
					_data = new int[this.Length];

				return _data;
			}
		}

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

		public uint Length
		{
			get { return (uint)(this.MaxInputValue - this.MinInputValue + 1); }
		}

		public abstract void Create();

		public void Clear()
		{
			_data = null;
		}

		#endregion


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
