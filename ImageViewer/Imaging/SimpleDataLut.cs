#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract base class for <see cref="IDataLut"/>s whose size and data
	/// do not change.
	/// </summary>
	[Cloneable]
	public class SimpleDataLut : DataLut
	{
		[CloneIgnore]
		private readonly int[] _data;
		private readonly string _key;
		private readonly string _description;
		private readonly int _firstMappedPixelValue;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SimpleDataLut(int firstMappedPixelValue, int[] data, int minOutputValue, int maxOutputValue, string key, string description)
		{
			Platform.CheckForNullReference(data, "data");
			Platform.CheckForEmptyString(key, "key");
			Platform.CheckForEmptyString(description, "description");

			_data = data;

			_key = key;
			_description = description;

			_firstMappedPixelValue = firstMappedPixelValue;

			base.MinInputValue = _firstMappedPixelValue;
			base.MaxInputValue = LastMappedPixelValue;
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
		/// dependent upon the actual image to which it belongs.
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
		/// Gets the first mapped pixel value.
		/// </summary>
		public sealed override int FirstMappedPixelValue
		{
			get { return _firstMappedPixelValue; }
		}

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public sealed override int LastMappedPixelValue
		{
			get { return _firstMappedPixelValue + _data.Length - 1; }
		}

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
