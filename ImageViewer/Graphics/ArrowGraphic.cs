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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arrow graphic.
	/// </summary>
	[Cloneable]
	public class ArrowGraphic : CompositeGraphic, ILineSegmentGraphic
	{
		[CloneIgnore]
		private LinePrimitive _shaft;

		[CloneIgnore]
		private InvariantArrowheadGraphic _arrowhead;

		private event EventHandler<PointChangedEventArgs> _startPointChanged;
		private event EventHandler<PointChangedEventArgs> _endPointChanged;

		private bool _visible = true;
		private bool _showArrowhead = true;

		/// <summary>
		/// Constructs an arrow graphic.
		/// </summary>
		public ArrowGraphic()
		{
			Initialize();
		}

		/// <summary>
		/// Constructs a arrow graphic with an optional arrowhead.
		/// </summary>
		/// <param name="showArrow">A value indicating if the arrowhead should be shown.</param>
		public ArrowGraphic(bool showArrow) : this()
		{
			_showArrowhead = showArrow;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ArrowGraphic(ArrowGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			if (_shaft == null)
			{
				base.Graphics.Add(_shaft = new LinePrimitive());
			}

			if (_arrowhead == null)
			{
				base.Graphics.Add(_arrowhead = new InvariantArrowheadGraphic());
				_arrowhead.Visible = _showArrowhead;
			}

			_shaft.Pt1Changed += OnShaftPt1Changed;
			_shaft.Pt2Changed += OnShaftPt2Changed;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_shaft = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is LinePrimitive; }) as LinePrimitive;
			_arrowhead = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is InvariantArrowheadGraphic; }) as InvariantArrowheadGraphic;

			Initialize();
		}

		/// <summary>
		/// Gets or sets the tail endpoint of the arrow.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF StartPoint
		{
			get { return _shaft.Pt1; }
			set
			{
				_shaft.Pt1 = value;
				UpdateArrowheadAngle();
			}
		}

		/// <summary>
		/// Gets or sets the tip endpoint of the arrow.
		/// </summary>
		/// <remarks>
		/// <para>This point is the location to which the arrow points.</para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.</para>
		/// </remarks>
		public PointF EndPoint
		{
			get { return _shaft.Pt2; }
			set
			{
				_shaft.Pt2 = _arrowhead.Point = value;
				UpdateArrowheadAngle();
			}
		}

		/// <summary>
		/// Event fired when the value of <see cref="StartPoint"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> StartPointChanged
		{
			add { _startPointChanged += value; }
			remove { _startPointChanged -= value; }
		}

		/// <summary>
		/// Event fired when the value of <see cref="EndPoint"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> EndPointChanged
		{
			add { _endPointChanged += value; }
			remove { _endPointChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the line style of the arrowhead.
		/// </summary>
		public LineStyle ArrowheadLineStyle
		{
			get { return _arrowhead.LineStyle; }
			set { _arrowhead.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the arrowhead should be visible.
		/// </summary>
		public bool ShowArrowhead
		{
			get { return _showArrowhead; }
			set
			{
				if (_showArrowhead != value)
				{
					_showArrowhead = value;
					UpdateArrowheadVisibility();
				}
			}
		}

		/// <summary>
		/// Gets or sets the colour of the arrow.
		/// </summary>
		public Color Color
		{
			get { return _shaft.Color; }
			set { _shaft.Color = _arrowhead.Color = value; }
		}

		/// <summary>
		/// Gets or sets the line style of the shaft of the arrow.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _shaft.LineStyle; }
			set { _shaft.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the arrow is visible.
		/// </summary>
		public override bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = _shaft.Visible = value;
				UpdateArrowheadVisibility();
			}
		}

		/// <summary>
		/// Gets or sets the tail endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point1
		{
			get { return this.StartPoint; }
			set { this.StartPoint = value; }
		}

		/// <summary>
		/// Gets or sets the tip endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point2
		{
			get { return this.EndPoint; }
			set { this.EndPoint = value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point1Changed
		{
			add { this.StartPointChanged += value; }
			remove { this.StartPointChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point2Changed
		{
			add { this.EndPointChanged += value; }
			remove { this.EndPointChanged -= value; }
		}

		private void UpdateArrowheadAngle()
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// the arrowhead is invariant, but the angle part isn't! must be computed with source coordinates!
				_arrowhead.Angle = (int)Vector.SubtendedAngle(_shaft.Pt2, _shaft.Pt1, _shaft.Pt1 + new SizeF(1, 0));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
			UpdateArrowheadVisibility();
		}

		private void UpdateArrowheadVisibility()
		{
			_shaft.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				// if arrowhead option is true and the graphic is visible, only show arrowhead if line is long enough!
				if (_showArrowhead && _visible)
					_arrowhead.Visible = Vector.Distance(_shaft.Pt1, _shaft.Pt2) > _arrowhead.Length;
				else
					_arrowhead.Visible = false;
			}
			finally
			{
				_shaft.ResetCoordinateSystem();
			}
		}

		private void OnShaftPt1Changed(object sender, PointChangedEventArgs e)
		{
			EventsHelper.Fire(_startPointChanged, this, new PointChangedEventArgs(e.Point));
		}

		private void OnShaftPt2Changed(object sender, PointChangedEventArgs e)
		{
			EventsHelper.Fire(_endPointChanged, this, new PointChangedEventArgs(e.Point));
		}
	}
}