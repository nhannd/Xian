#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
    /// TODO (CR Nov 2011): IColorMap does not inherit from IDataLut, so should there
    /// just be a clean separation here, or should it inherit?
    
	/// <summary>
	/// Basic implementation of an <see cref="IColorMap"/> whose size and data do not change.
	/// </summary>
	[Cloneable]
	public class SimpleColorMap : SimpleDataLut, IColorMap
	{
		private const string _exceptionMinOutputValue = "A minimum output value does not exist for a color map.";
		private const string _exceptionMaxOutputValue = "A maximum output value does not exist for a color map.";

		/// <summary>
		/// Initializes a new instance of <see cref="SimpleColorMap"/>.
		/// </summary>
		/// <param name="firstMappedPixelValue">The value of the first mapped pixel in the lookup table.</param>
		/// <param name="lutData">The lookup table mapping input pixel values to 32-bit ARGB colors.</param>
		/// <param name="key">A key suitable for identifying the color map.</param>
		/// <param name="description">A description of the color map.</param>
		public SimpleColorMap(int firstMappedPixelValue, int[] lutData, string key, string description)
			: base(firstMappedPixelValue, lutData, 0, 0, key, description) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected SimpleColorMap(SimpleColorMap source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates a deep-copy of the <see cref="IColorMap"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IColorMap"/> implementations may return NULL from this method when appropriate.	
		/// </remarks>
		public new IColorMap Clone()
		{
			return base.Clone() as IColorMap;
		}

		/// <summary>
		/// This property is not applicable for this class.
		/// </summary>
		/// <exception cref="InvalidOperationException">Always thrown.</exception>
		public override sealed int MinOutputValue
		{
			get { throw new InvalidOperationException(_exceptionMinOutputValue); }
			protected set { throw new InvalidOperationException(_exceptionMinOutputValue); }
		}

		/// <summary>
		/// This property is not applicable for this class.
		/// </summary>
		/// <exception cref="InvalidOperationException">Always thrown.</exception>
		public override sealed int MaxOutputValue
		{
			get { throw new InvalidOperationException(_exceptionMaxOutputValue); }
			protected set { throw new InvalidOperationException(_exceptionMaxOutputValue); }
		}
	}
}