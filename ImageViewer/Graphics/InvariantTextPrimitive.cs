using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A text <see cref="InvariantPrimitive"/>.
	/// </summary>
	/// <remarks>
	/// An <see cref="InvariantTextPrimitive"/> is both size invariant and
	/// rotation invariant.  That is, text always appears the same size irrespective of
	/// the zoom, and <i>always</i> appears left to right irrespective of the rotation.
	/// </remarks>
	public class InvariantTextPrimitive : InvariantPrimitive
	{
		private string _text;
		private float _sizeInPoints;
		private string _font;
		private SizeF _dimensions;
		private event EventHandler<RectangleChangedEventArgs> _boundingBoxChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantTextPrimitive"/>.
		/// </summary>
		public InvariantTextPrimitive()
		{
			this.SizeInPoints = 10;
			this.Font = "Arial";
		}

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantTextPrimitive"/> with
		/// the specified text.
		/// </summary>
		/// <param name="text"></param>
		public InvariantTextPrimitive(string text) : this()
		{
			_text = text;
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		/// Gets or sets the size in points.
		/// </summary>
		/// <remarks>
		/// Default value is 10 points.
		/// </remarks>
		public float SizeInPoints
		{
			get { return _sizeInPoints; }
			set { _sizeInPoints = value; }
		}

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <remarks>
		/// Default value is "Arial".
		/// </remarks>
		public string Font
		{
			get { return _font; }
			set { _font = value; }
		}

		/// <summary>
		/// Gets or sets the dimensions of text's bounding box.
		/// </summary>
		/// <remarks>
		/// <see cref="Dimensions"/> is not determined by <see cref="InvariantTextPrimitive"/>,
		/// but by the implementation of <see cref="IRenderer"/>, since the measurement of
		/// the text size is ultimately platform dependent.  It is up to the implementor
		/// of <see cref="IRenderer"/> to set this property.  For example, a GDI+ version of
		/// <see cref="IRenderer"/> might use <see cref="System.Drawing.Graphics.MeasureString"/> to
		/// determine the text dimensions.
		/// </remarks>
		public SizeF Dimensions
		{
			get 
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _dimensions;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return this.SpatialTransform.ConvertToDestination(_dimensions);
				}
			}
			set 
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_dimensions = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_dimensions = this.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_boundingBoxChangedEvent, this, new RectangleChangedEventArgs(this.BoundingBox));
			}
		}

		/// <summary>
		/// Gets the bounding box of the text in source or destination coordinates.  The centre of the 
		/// bounding box should be <see cref="AnchorPoint"/>.
		/// </summary>
		public RectangleF BoundingBox
		{
			get
			{
				float halfWidth = this.Dimensions.Width / 2F;
				float halfHeight = this.Dimensions.Height / 2F;

				return RectangleF.FromLTRB(this.AnchorPoint.X - halfWidth, this.AnchorPoint.Y - halfHeight, this.AnchorPoint.X + halfWidth, this.AnchorPoint.Y + halfHeight);
			}
		}

		/// <summary>
		/// Occurs when the <see cref="BoundingBox"/> property changes.
		/// </summary>
		public event EventHandler<RectangleChangedEventArgs> BoundingBoxChanged
		{
			add { _boundingBoxChangedEvent += value; }
			remove { _boundingBoxChangedEvent -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantTextPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantTextPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is within the 
		/// text's <see cref="BoundingBox"/>.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			PointF pointF = new PointF(point.X, point.Y);

			this.CoordinateSystem = CoordinateSystem.Destination;

			bool hit = this.BoundingBox.Contains(pointF);

			this.ResetCoordinateSystem();

			return hit;
		}
	}
}
