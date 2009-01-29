using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arrowhead graphic with fixed screen size.
	/// </summary>
	/// <remarks>
	/// An <see cref="InvariantArrowheadGraphic"/> is a graphic whose orientation and
	/// position can be fixed relative to the source coordinate system, but whose
	/// size is fixed in the destination coordinate system.
	/// </remarks>
	[Cloneable(true)]
	public class InvariantArrowheadGraphic : CompositeGraphic, IVectorGraphic
	{
		[CloneIgnore]
		private InvariantLinePrimitive _sideL;

		[CloneIgnore]
		private InvariantLinePrimitive _sideR;

		private PointF _point = PointF.Empty;
		private float _height = 15f;
		private float _widthAngle = 30f;
		private float _angle = 0f;

		/// <summary>
		/// Constructs a new arrowhead graphic with fixed screen size.
		/// </summary>
		public InvariantArrowheadGraphic()
		{
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected InvariantArrowheadGraphic(InvariantArrowheadGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			if (_sideL == null)
			{
				base.Graphics.Add(_sideL = new InvariantLinePrimitive());
				_sideL.InvariantBottomRight = PointF.Empty;
			}

			if (_sideR == null)
			{
				base.Graphics.Add(_sideR = new InvariantLinePrimitive());
				_sideR.InvariantBottomRight = PointF.Empty;
			}

			RecomputeArrow();
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			IList<IGraphic> lines = CollectionUtils.Select(base.Graphics,
			                                               delegate(IGraphic test) { return test is InvariantLinePrimitive; });

			_sideL = lines[0] as InvariantLinePrimitive;
			_sideR = lines[1] as InvariantLinePrimitive;

			Initialize();
		}

		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		public Color Color
		{
			get { return _sideL.Color; }
			set { _sideL.Color = _sideR.Color = value; }
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _sideL.LineStyle; }
			set { _sideL.LineStyle = _sideR.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets the location that the arrowhead points to.
		/// </summary>
		/// <remarks>
		/// <para>This property specifies the point that the arrowhead points to,
		/// as well as the point to which it is fixed. As the parent zoom changes,
		/// the screen size of arrowhead will remain the same, and this point will
		/// remain pointing to the same location in the parent graphic.</para>
		/// <para>This property can be specified in either source or destination
		/// coordinates depending on the value of
		/// <see cref="InvariantArrowheadGraphic.CoordinateSystem"/>.</para>
		/// </remarks>
		public PointF Point
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point;
				else
					return base.SpatialTransform.ConvertToDestination(_point);
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Destination)
					value = base.SpatialTransform.ConvertToSource(value);

				if (!FloatComparer.AreEqual(_point, value))
				{
					_point = value;
					RecomputeArrow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle in degrees in which the arrowhead points.
		/// </summary>
		/// <remarks>
		/// The angle is specified in terms of the standard polar coordinate axes relative to the parent's <see cref="ISpatialTransform"/>
		/// (i.e. counterclockwise from the positive X-axis).
		/// </remarks>
		public float Angle
		{
			get { return _angle; }
			set
			{
				if (!FloatComparer.AreEqual(_angle, value))
				{
					_angle = value;
					RecomputeArrow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the height of the arrowhead.
		/// </summary>
		/// <remarks>
		/// The height of the arrowhead is the altitude along the shaft of the triangle formed by the arrowhead.
		/// </remarks>
		public float Height
		{
			get { return _height; }
			set
			{
				if (!FloatComparer.AreEqual(_height, value))
				{
					_height = value;
					RecomputeArrow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the width angle of the arrowhead in degrees.
		/// </summary>
		/// <remarks>
		/// The width angle is the inner angle formed by the two sides of the arrowhead.
		/// </remarks>
		public float WidthAngle
		{
			get { return _widthAngle; }
			set
			{
				if (!FloatComparer.AreEqual(_widthAngle, value))
				{
					_widthAngle = value;
					RecomputeArrow();
				}
			}
		}

		/// <summary>
		/// Recomputes the shape and positioning of the arrowhead's line graphics.
		/// </summary>
		protected void RecomputeArrow()
		{
			float height = (float) (_height*Math.Tan(Math.PI*_widthAngle/360));
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				PointF pt = this.Point;

				_sideL.AnchorPoint = pt;
				_sideL.InvariantTopLeft = new PointF(-_height, -height);
				_sideL.SpatialTransform.CenterOfRotationXY = pt;
				_sideL.SpatialTransform.RotationXY = (int) _angle;

				_sideR.AnchorPoint = pt;
				_sideR.InvariantTopLeft = new PointF(-_height, height);
				_sideR.SpatialTransform.CenterOfRotationXY = pt;
				_sideR.SpatialTransform.RotationXY = (int) _angle;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}
	}
}