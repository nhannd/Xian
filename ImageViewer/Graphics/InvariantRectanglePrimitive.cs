#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A rectangular <see cref="InvariantPrimitive"/>.
	/// </summary>
	[Cloneable(true)]
    /// TODO (CR Oct 2011): Should these actually be serializable given that presentation states won't show them invariant.
    [DicomSerializableGraphicAnnotation(typeof(RectangleGraphicAnnotationSerializer))]
	public class InvariantRectanglePrimitive : InvariantBoundablePrimitive
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InvariantRectanglePrimitive"/>.
		/// </summary>
		public InvariantRectanglePrimitive()
		{
		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantRectanglePrimitive"/> 
		/// at a given point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			bool result = RectanglePrimitive.HitTest(
				this.SpatialTransform.ConvertToSource(point), 
				this.Rectangle, 
				this.SpatialTransform);
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point on the <see cref="Graphic"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			return RectanglePrimitive.GetClosestPoint(point, this.Rectangle);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool Contains(PointF point)
		{
			return this.Rectangle.Contains(point);
		}
	}
}
