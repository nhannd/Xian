using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arrow graphic.
	/// </summary>
	[Cloneable(true)]
	public class ArrowGraphic : CompositeGraphic, ILineSegmentGraphic
	{
		[CloneIgnore]
		private LinePrimitive _shaft;

		[CloneIgnore]
		private InvariantArrowheadGraphic _arrowhead;

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
			_arrowhead.Visible = showArrow;
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
			}
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
			get { return _arrowhead.Visible; }
			set { _arrowhead.Visible = value & this.Visible; }
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
		/// One endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Pt1
		{
			get { return this.StartPoint; }
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
		}

		private void UpdateArrowheadAngle()
		{
			_arrowhead.Angle = (int) Vector.SubtendedAngle(_shaft.Pt2, _shaft.Pt1, _shaft.Pt1 + new SizeF(1, 0));
		}
	}
}