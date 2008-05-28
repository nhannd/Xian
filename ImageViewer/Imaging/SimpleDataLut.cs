using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract base class for <see cref="IDataLut"/>s whose size and data
	/// do not change.
	/// </summary>
	[Cloneable(true)]
	public class SimpleDataLut : DataLut
	{
		[CloneIgnore]
		private readonly int[] _data;
		private readonly string _key;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SimpleDataLut(int minInputValue, int[] data, int minOutputValue, int maxOutputValue, string key, string description)
		{
			Platform.CheckForNullReference(data, "data");
			Platform.CheckForEmptyString(key, "key");
			Platform.CheckForEmptyString(description, "description");

			_data = data;

			_key = key;
			_description = description;

			base.MinInputValue = minInputValue;
			base.MinOutputValue = minOutputValue;
			base.MaxOutputValue = maxOutputValue;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected SimpleDataLut(SimpleDataLut source, ICloningContext context)
		{
			context.CloneFields(source, this);

			//clone the actual buffer
			_data = (int[])source._data.Clone();
		}

		/// <summary>
		/// Gets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MinInputValue
		{
			get { return base.MinInputValue; }
			set { }
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MaxInputValue
		{
			get { return base.MinInputValue + _data.Length - 1; }
			set { }
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MinOutputValue
		{
			get { return base.MinOutputValue; }
			protected set
			{
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MaxOutputValue
		{
			get { return base.MaxOutputValue; }
			protected set
			{
			}
		}

		/// <summary>
		/// Gets a string key that identifies this particular Lut's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="ComposedLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public override string GetKey()
		{
			return _key;
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return _description;
		}

		#region IDataLut Members

		/// <summary>
		/// Gets the lut data.
		/// </summary>
		public override int[] Data
		{
			get { return _data; }
		}

		#endregion
	}
}
