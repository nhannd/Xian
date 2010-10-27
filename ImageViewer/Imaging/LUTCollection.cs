#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A collection of <see cref="IComposableLut"/> objects.
	/// </summary>
	public class LutCollection : ObservableList<IComposableLut>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		internal LutCollection()
		{
		}

		internal void Validate()
		{
			int count = 0;

			// Check for null LUTs
			foreach (IComposableLut lut in this)
			{
				++count;
				if (lut == null)
					throw new InvalidOperationException(SR.ExceptionLUTNotAdded);
			}

			if (count == 0) //do this instead of accessing Count b/c it can be expensive.
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Verify that the input range of the nth LUT is equal to the output
			// range of the n-1th LUT.
			for (int i = 1; i < count; i++)
			{
				IComposableLut curLut = this[i];
				IComposableLut prevLut = this[i - 1];

				if (prevLut.MinOutputValue != curLut.MinInputValue ||
					prevLut.MaxOutputValue != curLut.MaxInputValue)
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}
		}

		internal void SyncMinMaxValues()
		{
			int count = Count;
			for (int i = 1; i < count; ++i)
			{
				IComposableLut curLut = this[i];
				IComposableLut prevLut = this[i - 1];

				curLut.MinInputValue = prevLut.MinOutputValue;
				curLut.MaxInputValue = prevLut.MaxOutputValue;
			}
		}
	}
}
