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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Base class for validating <see cref="SpatialTransform"/> objects.
	/// </summary>
	/// <remarks>
	/// It is not always desirable to allow an <see cref="IGraphic"/> to be transformed
	/// in arbitrary ways.  For example, at present, images can only be rotated in
	/// 90 degree increments.  This class allows a validation policy to be defined on a
	/// per graphic basis.  If validation fails, an <see cref="ArgumentException"/> is thrown.
	/// </remarks>
	[Cloneable(true)]
	public abstract class SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransformValidationPolicy"/>.
		/// </summary>
		protected SpatialTransformValidationPolicy()
		{

		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// Implementors should throw an <see cref="ArgumentException"/> if validation fails.
		/// </remarks>
		public abstract void Validate(ISpatialTransform transform);
	}
}
