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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A polyline graphic.
	/// </summary>
	/// <remarks>
	/// A polyline is specified by a series of <i>n</i> anchor points, which
	/// are connected with a series of <i>n-1</i> line segments.
	/// </remarks>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (PolylineGraphicAnnotationSerializer))]
	public class PolylineGraphic : CompositeGraphic, IPointsGraphic
	{
		#region Private fields

		private Color _color = Color.Yellow;
		private LineStyle _lineStyle = LineStyle.Solid;
		private bool _roiClosedOnly = false;

		[CloneIgnore]
		private LinesComposite _lines;

		[CloneIgnore]
		private readonly PointsList _points;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="PolylineGraphic"/>.
		/// </summary>
		public PolylineGraphic()
		{
			_points = new PointsList(this);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PolylineGraphic"/>.
		/// </summary>
		/// <param name="roiClosedOnly">
		/// True if this graphic should only be treated as a
		/// closed polygon for the purposes of ROI computation
		/// (<seealso cref="CreateRoi"/>).
		/// </param>
		public PolylineGraphic(bool roiClosedOnly) : this()
		{
			_roiClosedOnly = roiClosedOnly;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected PolylineGraphic(PolylineGraphic source, ICloningContext context)
			: base()
		{
			context.CloneFields(source, this);
			_points = new PointsList(source._points, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_lines = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is LinesComposite; }) as LinesComposite;

			Initialize();
		}

		private void Initialize()
		{
			if (_lines == null)
				this.Graphics.Add(_lines = new LinesComposite());

			_points.PointAdded += OnPointsItemAdded;
			_points.PointChanged += OnPointsItemChanged;
			_points.PointRemoved += OnPointsItemRemoved;
			_points.PointsCleared += OnPointsCleared;
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="PolylineGraphic"/>.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					this.OnColorChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the line style of the <see cref="PolylineGraphic"/>.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _lineStyle; }
			set
			{
				if (_lineStyle != value)
				{
					_lineStyle = value;
					this.OnLineStyleChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the <see cref="CreateRoi"/> method
		/// should assume the shape should always be a closed polygon.
		/// </summary>
		/// <remarks>
		/// If True, then <see cref="CreateRoi"/> will only ever return
		/// <see cref="PolygonalRoi"/> objects (or null, if the shape is not a closed polygon).
		/// If False, then the method will return <see cref="PolygonalRoi"/> or <see cref="LinearRoi"/>
		/// depending on the current shape (or null, if the shape is neither a closed polygon nor
		/// a line).
		/// </remarks>
		public virtual bool RoiClosedOnly
		{
			get { return _roiClosedOnly; }
			set { _roiClosedOnly = value; }
		}

		/// <summary>
		/// Gets the vertices of the polyline.
		/// </summary>
		public IPointsList Points
		{
			get { return _points; }
		}

		/// <summary>
		/// Performs a hit test on the polyline.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			if (_points.Count == 1)
				return Vector.Distance(point, _points[0]) < VectorGraphic.HitTestDistance;

			foreach (IGraphic graphic in this.Graphics)
			{
				if (graphic.HitTest(point))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses all the bounding boxes of the child graphics in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get
			{
				if (_points.Count == 0)
					return RectangleF.Empty;
				return RectangleUtilities.ComputeBoundingRectangle(_points);
			}
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
			base.Move(delta);

			for (int n = 0; n < _points.Count; n++)
				_points[n] += delta;
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
		/// <para>Calling <see cref="CompositeGraphic.GetClosestPoint"/> will recursively call <see cref="CompositeGraphic.GetClosestPoint"/>
		/// on <see cref="Graphic"/> objects in the subtree and return the closest result.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			if (_points.Count == 1)
				return _points[0];

			return base.GetClosestPoint(point);
		}

		/// <summary>
		/// Creates an object describing the region of interest on the <see cref="Graphic.ParentPresentationImage"/> selected by this <see cref="Graphic"/>.
		/// </summary>
		/// <remarks>
		/// Graphic objects that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="Roi"/> describing this region of interest, or null if the graphic does not describe a region of interest.</returns>
		public override Roi CreateRoi()
		{
			if (_points.Count == 2 && !_roiClosedOnly)
			{
				return new LinearRoi(this);
			}
			else if (_points.IsClosed)
			{
				return new PolygonalRoi(this);
			}
			return base.CreateRoi();
		}

		private void OnPointsItemAdded(object sender, IndexEventArgs e)
		{
			if (_points.Count >= 2)
			{
				LinePrimitive line = new LinePrimitive();
				line.Color = _color;
				line.LineStyle = _lineStyle;
				line.CoordinateSystem = this.CoordinateSystem;
				try
				{
					if (e.Index == _points.Count - 1)
					{
						_lines.Graphics.Add(line);
						line.Pt1 = _points[e.Index - 1];
						line.Pt2 = _points[e.Index];
					}
					else
					{
						_lines.Graphics.Insert(e.Index, line);
						line.Pt1 = _points[e.Index];
						line.Pt2 = _points[e.Index + 1];

						if (e.Index > 0)
							((LinePrimitive) _lines.Graphics[e.Index - 1]).Pt2 = _points[e.Index];
					}
				}
				finally
				{
					line.ResetCoordinateSystem();
				}
			}
			base.NotifyVisualStateChanged("Points");
		}

		private void OnPointsItemRemoved(object sender, IndexEventArgs e)
		{
			if (_lines.Graphics.Count > 0)
			{
				if (e.Index == _points.Count)
				{
					_lines.Graphics.RemoveAt(e.Index - 1);
				}
				else if (e.Index > 0)
				{
					_lines.Graphics.RemoveAt(e.Index);
					((LinePrimitive) _lines.Graphics[e.Index - 1]).Pt2 = _points[e.Index];
				}
				else
				{
					_lines.Graphics.RemoveAt(e.Index);
				}
			}
			base.NotifyVisualStateChanged("Points");
		}

		private void OnPointsItemChanged(object sender, IndexEventArgs e)
		{
			if (_lines.Graphics.Count > 0)
			{
				if (e.Index < _points.Count - 1)
				{
					((LinePrimitive) _lines.Graphics[e.Index]).Pt1 = _points[e.Index];
				}

				if (e.Index > 0)
				{
					((LinePrimitive) _lines.Graphics[e.Index - 1]).Pt2 = _points[e.Index];
				}
			}
			base.NotifyVisualStateChanged("Points");
		}

		private void OnPointsCleared(object sender, EventArgs e)
		{
			_lines.Graphics.Clear();
			base.NotifyVisualStateChanged("Points");
		}

		/// <summary>
		/// Called when the value of the <see cref="Color"/> property changes.
		/// </summary>
		protected virtual void OnColorChanged()
		{
			foreach (LinePrimitive line in _lines.Graphics)
				line.Color = _color;
		}

		/// <summary>
		/// Called when the value of the <see cref="LineStyle"/> property changes.
		/// </summary>
		protected virtual void OnLineStyleChanged()
		{
			foreach (LinePrimitive line in _lines.Graphics)
				line.LineStyle = _lineStyle;
		}

		[Cloneable(true)]
		private class LinesComposite : CompositeGraphic
		{
			public LinesComposite() : base() {}
		}
	}
}