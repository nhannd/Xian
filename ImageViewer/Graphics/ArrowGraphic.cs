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

		public event EventHandler<PointChangedEventArgs> StartPointChanged
		{
			add { _startPointChanged += value; }
			remove { _startPointChanged -= value; }
		}

		public event EventHandler<PointChangedEventArgs> EndPointChanged
		{
			add { _endPointChanged += value; }
			remove { _endPointChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the line style to be used on the arrowhead.
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
		/// Gets or sets the colour.
		/// </summary>
		public Color Color
		{
			get { return _shaft.Color; }
			set { _shaft.Color = _arrowhead.Color = value; }
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _shaft.LineStyle; }
			set { _shaft.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this arrow is visible.
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
		/// Gets the point on the arrow closest to the specified <paramref name="point"/>.
		/// </summary>
		public PointF GetClosestPoint(PointF point)
		{
			PointF pointS = new PointF();
			double distanceS = Vector.DistanceFromPointToLine(point, _shaft.Pt1, _shaft.Pt2, ref pointS);
			PointF pointA = _arrowhead.GetClosestPoint(point);
			double distanceA = Vector.Distance(point, pointA);
			return distanceS < distanceA ? pointS : pointA;
		}

		/// <summary>
		/// One endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt1
		{
			get { return this.StartPoint; }
			set { this.StartPoint = value; }
		}

		/// <summary>
		/// The other endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt2
		{
			get { return this.EndPoint; }
			set { this.EndPoint = value; }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt1Changed
		{
			add { this.StartPointChanged += value; }
			remove { this.StartPointChanged -= value; }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Pt2Changed
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
					_arrowhead.Visible = Vector.Distance(_shaft.Pt1, _shaft.Pt2) > _arrowhead.Height;
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