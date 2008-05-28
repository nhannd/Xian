using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Base implementation for <see cref="IDataLut"/>s.
	/// </summary>
	/// <remarks>
	/// Normally, you should not have to inherit directly from this class.
	/// <see cref="SimpleDataLut"/> or <see cref="GeneratedDataLut"/> should cover
	/// any common use cases.
	/// </remarks>
	[Cloneable(true)]
	public abstract class DataLut : ComposableLut, IDataLut
	{
		private int _minInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DataLut()
		{
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (value == _minInputValue)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set
			{
				if (_minOutputValue == value)
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set
			{
				if (value == _maxOutputValue)
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public override int this[int index]
		{
			get
			{
				Platform.CheckMemberIsSet(Data, "Data");
				Platform.CheckTrue(Data.Length == Length, "Data Lut length check");

				if (index <= MinInputValue)
					return Data[0];
				else if (index >= this.MaxInputValue)
					return Data[this.Length - 1];
				else
					return Data[index - this.MinInputValue];
			}
			protected set
			{
				if (index < this.MinInputValue || index > this.MaxInputValue)
					return;

				this.Data[index - this.MinInputValue] = value;
			}
		}

		///<summary>
		/// Gets the length of <see cref="Data"/>.
		///</summary>
		/// <remarks>
		/// The reason for this member's existence is that <see cref="Data"/> may
		/// not yet exist; this value is based solely on <see cref="IComposableLut.MinInputValue"/>
		/// and <see cref="IComposableLut.MaxInputValue"/>.
		/// </remarks>
		public uint Length
		{
			get
			{
				return (uint)(1 + MaxInputValue - MinInputValue);
			}
		}

		#region IDataLut Members

		/// <summary>
		/// Gets the lut data.
		/// </summary>
		public abstract int[] Data { get; }

		#endregion
	}
}
