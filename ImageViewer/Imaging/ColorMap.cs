#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: one of these days, make this a simple data lut or have 2 types of color map.  ColorMap is a generated data lut, and this one doesn't need to be.
	/// <summary>
	/// Implements a Color Map as a LUT.
	/// </summary>
	/// <remarks>
	/// The values in the LUT represent ARGB values that are used 
	/// by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/> to display the image.
	/// </remarks>
	/// <seealso cref="GeneratedDataLut"/>
	public abstract class ColorMap : GeneratedDataLut, IColorMap
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ColorMap()
		{
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown always.</exception>
		public sealed override int MinOutputValue
		{
			get
			{
				throw new InvalidOperationException("A color map cannot have a minimum output value.");
			}
			protected set
			{
				throw new InvalidOperationException("A color map cannot have a minimum output value.");
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown always.</exception>
		public sealed override int MaxOutputValue
		{
			get
			{
				throw new InvalidOperationException("A color map cannot have a maximum output value.");
			}
			protected set
			{
				throw new InvalidOperationException("A color map cannot have a maximum output value.");
			}
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// You should not have to override this method.
		/// </remarks>
		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetType().ToString());
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
	}
}
