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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
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
		private event EventHandler<ListEventArgs<PointF>> _anchorPointChangedEvent;
		private event EventHandler _anchorPointsChangedEvent;

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
		/// (<seealso cref="PolylineGraphic.CreateRoiInformation"/>).
		/// </param>
		public PolylineGraphic(bool roiClosedOnly) : this()
		{
			_roiClosedOnly = roiClosedOnly;
		}

		protected PolylineGraphic(PolylineGraphic source, ICloningContext context)
			: base()
		{
			context.CloneFields(source, this);
			_points = source._points.Clone(this);
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

			_points.PointAdded += new EventHandler<IndexEventArgs>(OnPointsItemAdded);
			_points.PointChanged += new EventHandler<IndexEventArgs>(OnPointsItemChanged);
			_points.PointRemoved += new EventHandler<IndexEventArgs>(OnPointsItemRemoved);
			_points.PointsCleared += new EventHandler(OnPointsCleared);
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
		/// Gets or sets a value indicating that the <see cref="PolylineGraphic.CreateRoiInformation"/> method
		/// should assume the shape should always be a closed polygon.
		/// </summary>
		/// <remarks>
		/// If True, then <see cref="PolylineGraphic.CreateRoiInformation"/> will only ever return
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
		/// Gets the number of anchor points in the <see cref="PolylineGraphic"/>.
		/// </summary>
		public int Count
		{
			get { return _points.Count; }
		}

		/// <summary>
		/// Occurs when an anchor point has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> AnchorPointChangedEvent
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the list of anchor points has changed (inserted, removed or cleared)
		/// </summary>
		public event EventHandler AnchorPointsChangedEvent
		{
			add { _anchorPointsChangedEvent += value; }
			remove { _anchorPointsChangedEvent -= value; }
		}

		/// <summary>
		/// Adds a new anchor point to the <see cref="PolylineGraphic"/>.
		/// </summary>
		/// <param name="point">The anchor point to be inserted.</param>
		/// <remarks>
		/// The anchor point should be in either source or destination coordinates depending
		/// on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public void Add(PointF point)
		{
			_points.Add(point);
		}

		/// <summary>
		/// Inserts a new anchor point to the <see cref="PolylineGraphic"/>.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert the anchor point.</param>
		/// <param name="point">The anchor point to be inserted.</param>
		/// <remarks>
		/// The anchor point should be in either source or destination coordinates depending
		/// on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public void Insert(int index, PointF point)
		{
			_points.Insert(index, point);
		}

		/// <summary>
		/// Removes an anchor point from the <see cref="PolylineGraphic"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the anchor point to remove.</param>
		public void RemoveAt(int index)
		{
			_points.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the location of the specified anchor point.
		/// </summary>
		/// <param name="index">The zero-based index of the anchor point.</param>
		/// <remarks>
		/// This property returns points in either source or destination coordinates depending
		/// on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public PointF this[int index]
		{
			get { return _points[index]; }
			set { _points[index] = value; }
		}

		/// <summary>
		/// Removes all anchor points from the <see cref="PolylineGraphic"/>.
		/// </summary>
		public void Clear()
		{
			_points.Clear();
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

		public override RectangleF BoundingBox
		{
			get
			{
				if (_points.Count == 0)
					return RectangleF.Empty;
				return RectangleUtilities.ComputeBoundingRectangle(_points);
			}
		}

		public override void Move(SizeF delta)
		{
			base.Move(delta);

			_points.SuspendEvents();
			for (int n = 0; n < _points.Count; n++)
				_points[n] += delta;
			_points.ResumeEvents();

			NotifyListeners();
		}

		public override PointF GetClosestPoint(PointF point)
		{
			if (_points.Count == 1)
				return _points[0];

			return base.GetClosestPoint(point);
		}

		public override Roi CreateRoiInformation()
		{
			if (_points.Count == 2 && !_roiClosedOnly)
			{
				return new LinearRoi(this);
			}
			else if (_points.IsClosed)
			{
				return new PolygonalRoi(this);
			}
			return base.CreateRoiInformation();
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

			NotifyListeners();
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

			NotifyListeners();
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

			NotifyListeners(e.Index, _points[e.Index]);
		}

		private void OnPointsCleared(object sender, EventArgs e)
		{
			_lines.Graphics.Clear();
			NotifyListeners();
		}

		protected virtual void OnColorChanged()
		{
			foreach (LinePrimitive line in _lines.Graphics)
				line.Color = _color;
		}

		protected virtual void OnLineStyleChanged()
		{
			foreach (LinePrimitive line in _lines.Graphics)
				line.LineStyle = _lineStyle;
		}

		private void NotifyListeners(int anchorPointIndex, PointF anchorPoint)
		{
			EventsHelper.Fire(_anchorPointChangedEvent, this, new ListEventArgs<PointF>(anchorPoint, anchorPointIndex));
			base.NotifyPropertyChanged("Points");
		}

		private void NotifyListeners()
		{
			EventsHelper.Fire(_anchorPointsChangedEvent, this, new EventArgs());
			base.NotifyPropertyChanged("Points");
		}

		[Cloneable(true)]
		private class LinesComposite : CompositeGraphic
		{
			public LinesComposite() : base() {}
		}

		#region IPointsGraphic Members

		event EventHandler<ListEventArgs<PointF>> IPointsGraphic.PointChanged
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		event EventHandler IPointsGraphic.PointsChanged
		{
			add { _anchorPointsChangedEvent += value; }
			remove { _anchorPointsChangedEvent -= value; }
		}

		int IPointsGraphic.IndexOfNextPoint(PointF point)
		{
			int index = 0;
			double best = double.MaxValue;
			PointF closestPoint = this.GetClosestPoint(point);
			PointF temp = PointF.Empty;
			for (int n = 0; n < _points.Count - 1; n++)
			{
				double distance = Vector.DistanceFromPointToLine(closestPoint, _points[n], _points[n + 1], ref temp);
				if (distance < best)
				{
					best = distance;
					index = n + 1;
				}
			}
			return index;
		}

		IList<PointF> IPointsGraphic.Points
		{
			get { return _points; }
		}

		#endregion
	}
}