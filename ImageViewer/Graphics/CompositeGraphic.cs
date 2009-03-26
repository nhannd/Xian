#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="Graphic"/> that can group other <see cref="Graphic"/> objects.
	/// </summary>
	[Cloneable(true)]
	public class CompositeGraphic : Graphic
	{
		private GraphicCollection _graphics;

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeGraphic"/>.
		/// </summary>
		public CompositeGraphic()
		{
		}

		/// <summary>
		/// Gets a collection of this <see cref="CompositeGraphic"/>'s child graphics.
		/// </summary>
		public GraphicCollection Graphics
		{
			get 
			{
				if (_graphics == null)
				{
					_graphics = new GraphicCollection();
					_graphics.ItemAdded += new EventHandler<ListEventArgs<IGraphic>>(OnGraphicAdded);
					_graphics.ItemRemoved += new EventHandler<ListEventArgs<IGraphic>>(OnGraphicRemoved);
				}

				return _graphics;
			}
		}

		//leave this override because the comments are important.

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CompositeGraphic"/> is visible.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="Visible"/> property will <b>not</b> recursively set the 
		/// <see cref="Visible"/> property for child objects in the subtree for reasons
		/// of preservation of state.  For example, if a <see cref="CompositeGraphic"/> at
		/// the very top of the scene graph had many child graphics whose visibility were
		/// dependent on, say, the position of the mouse, when the <see cref="CompositeGraphic"/>'s
		/// visibility changed, the visibility of the child objects would no longer be correct.
		/// Also, when a parent graphic's <see cref="Visible"/> property is false, it's children
		/// are not rendered regardless of the value of their <see cref="Visible"/> property.
		/// </remarks>
		public override bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}

		public override RectangleF BoundingBox
		{
			get
			{
				List<RectangleF> rectangles = new List<RectangleF>();
				foreach (IGraphic graphic in this.Graphics)
				{
					RectangleF rect = graphic.BoundingBox;
					if (!rect.Size.IsEmpty || !rect.Location.IsEmpty)
						rectangles.Add(rect);
				}
				if (rectangles.Count == 0)
					return RectangleF.Empty;
				return RectangleUtilities.ComputeBoundingRectangle(rectangles);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="CoordinateSystem"/> property will recursively set the 
		/// <see cref="CoordinateSystem"/> property for <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </remarks>
		public override CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set
			{
				base.CoordinateSystem = value;

				foreach (Graphic graphic in this.Graphics)
					graphic.CoordinateSystem = value;
			}
		}

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="CoordinateSystem"/> was last set.
		/// </para>
		/// <para>
		/// Calling <see cref="ResetCoordinateSystem"/> will recursively call
		/// <see cref="ResetCoordinateSystem"/> on <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </para>
		/// </remarks>
		public override void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();

			foreach (Graphic graphic in this.Graphics)
				graphic.ResetCoordinateSystem();
		}

		internal override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			foreach (Graphic graphic in this.Graphics)
				graphic.SetImageViewer(imageViewer);
		}

		internal override void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			base.SetParentPresentationImage(parentPresentationImage);

			foreach (Graphic graphic in this.Graphics)
				graphic.SetParentPresentationImage(parentPresentationImage);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="CompositeGraphic"/> at given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" any <see cref="Graphic"/>
		/// in the subtree, <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// Calling <see cref="HitTest"/> will recursively call <see cref="HitTest"/> on
		/// <see cref="Graphic"/> objects in the subtree.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			foreach (Graphic graphic in this.Graphics)
			{
				if (graphic.HitTest(point))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the point on the <see cref="CompositeGraphic"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// <para>Calling <see cref="GetClosestPoint"/> will recursively call <see cref="GetClosestPoint"/>
		/// on <see cref="Graphic"/> objects in the subtree and return the closest result.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			PointF result = PointF.Empty;
			double min = double.MaxValue;
			foreach (Graphic graphic in this.Graphics)
			{
				PointF pt = graphic.GetClosestPoint(point);
				double d = Vector.Distance(point, pt);
				if (min > d)
				{
					min = d;
					result = pt;
				}
			}
			return result;
		}

		/// <summary>
		/// Moves the <see cref="CompositeGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
 		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			foreach (Graphic graphic in this.Graphics)
				graphic.Move(delta);
		}

		/// <summary>
		/// Releases all resources used by this <see cref="CompositeGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Graphics.ItemAdded -= new EventHandler<ListEventArgs<IGraphic>>(OnGraphicAdded);
				this.Graphics.ItemRemoved -= new EventHandler<ListEventArgs<IGraphic>>(OnGraphicRemoved);

				foreach (Graphic graphic in this.Graphics)
					graphic.Dispose();
			}
		}

		private void OnGraphicAdded(object sender, ListEventArgs<IGraphic> e)
		{
			Graphic graphic = (Graphic)e.Item;

			graphic.SetParentGraphic(this);
			graphic.SetParentPresentationImage(this.ParentPresentationImage);
			graphic.SetImageViewer(this.ImageViewer);
			graphic.CoordinateSystem = this.CoordinateSystem;
		}

		private void OnGraphicRemoved(object sender, ListEventArgs<IGraphic> e)
		{
			Graphic graphic = (Graphic)e.Item;

			if (graphic is ISelectableGraphic)
				((ISelectableGraphic) graphic).Selected = false;
			if (graphic is IFocussableGraphic)
				((IFocussableGraphic) graphic).Focussed = false;

			graphic.SetParentGraphic(null);
			graphic.SetParentPresentationImage(null);
			graphic.SetImageViewer(null);
		}

		[CloneInitialize]
		private void Initialize(CompositeGraphic source, ICloningContext context)
		{
			foreach (IGraphic graphic in source.Graphics)
			{
				IGraphic clone = graphic.Clone();
				if (clone != null)
					this.Graphics.Add(clone);
			}
		}

		/// <summary>
		/// Enumerates the immediate child graphics of this <see cref="CompositeGraphic"/>.
		/// </summary>
		/// <param name="reverse">A value specifying that the enumeration should be carried out in reverse order.</param>
		/// <returns>An enumeration of child graphics.</returns>
		public IEnumerable<IGraphic> EnumerateChildGraphics(bool reverse)
		{
			if (reverse)
				return this.ReverseEnumerateChildGraphics();
			return this.Graphics;
		}

		private IEnumerable<IGraphic> ReverseEnumerateChildGraphics()
		{
			for (int graphicIndex = this.Graphics.Count - 1; graphicIndex >= 0; --graphicIndex)
			{
				yield return this.Graphics[graphicIndex];
			}
		}
	}
}


