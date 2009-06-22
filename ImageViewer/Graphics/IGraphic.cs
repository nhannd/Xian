#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a graphical object that can be rendered.
	/// </summary>
	public interface IGraphic : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's parent <see cref="IGraphic"/>.
		/// </summary>
		IGraphic ParentGraphic { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's associated 
		/// <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The associated <see cref="IPresentationImage"/> or <b>null</b>
		/// if the <see cref="IGraphic"/> is not yet part of the scene graph.</value>
		IPresentationImage ParentPresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="IGraphic"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IGraphic"/> is visible.
		/// </summary>
		bool Visible { get; set; }
		
		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		RectangleF BoundingBox { get; }

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="IGraphic.CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="IGraphic"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
		/// </remarks>
		CoordinateSystem CoordinateSystem { get; set; }

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's <see cref="SpatialTransform"/>.
		/// </summary>
		SpatialTransform SpatialTransform { get; }

		/// <summary>
		/// Gets or sets the name of this <see cref="IGraphic"/>.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Performs a hit test on the <see cref="IGraphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="IGraphic"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// It is up to the implementation of <see cref="IGraphic"/> to define what a "hit" is.
		/// </remarks>
		bool HitTest(Point point);

		/// <summary>
		/// Gets the point on the graphic closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.
		/// </remarks>
		PointF GetClosestPoint(PointF point);

		/// <summary>
		/// Moves the <see cref="IGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		void Move(SizeF delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="IGraphic.CoordinateSystem"/> property
		/// was last set.
		/// </para>
		/// </remarks>
		void ResetCoordinateSystem();

		/// <summary>
		/// Creates a deep copy of the graphic.
		/// </summary>
		/// <remarks>
		/// Graphic objects that are not cloneable may return null.
		/// </remarks>
		IGraphic Clone();

		/// <summary>
		/// Gets an object describing the region of interest on the <see cref="ParentPresentationImage"/> selected by this <see cref="IGraphic"/>.
		/// </summary>
		/// <remarks>
		/// Graphic objects that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="Roi"/> describing this region of interest, or null if the graphic does not describe a region of interest.</returns>
		Roi GetRoi();

		/// <summary>
		/// Occurs when a property is changed on a graphic, resulting in a change in the graphic's visual state.
		/// </summary>
		event VisualStateChangedEventHandler VisualStateChanged;
	}
}
