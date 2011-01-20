#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Implements an <see cref="ISpatialTransform"/> which is invariant in the destination coordinate system with respect to scale, flip and rotation.
	/// </summary>
	[Cloneable]
	public sealed class InvariantSpatialTransform : SpatialTransform
	{
		/// <summary>
		/// Initializes a new <see cref="InvariantSpatialTransform"/>.
		/// </summary>
		/// <param name="ownerGraphic">The graphic for which this <see cref="InvariantSpatialTransform"/> is being constructed.</param>
		public InvariantSpatialTransform(IGraphic ownerGraphic) : base(ownerGraphic) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private InvariantSpatialTransform(InvariantSpatialTransform source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Called by the base <see cref="SpatialTransform"/> to post-multiply an operation to the overall transformation matrix.
		/// </summary>
		protected override void CalculatePostTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Reset();
			cumulativeTransform.Translate(this.TranslationX, this.TranslationY);
		}

		/// <summary>
		/// Called by the base <see cref="SpatialTransform"/> to pre-multiply an operation to the overall transformation matrix.
		/// </summary>
		protected override void CalculatePreTransform(Matrix cumulativeTransform) {}
	}
}