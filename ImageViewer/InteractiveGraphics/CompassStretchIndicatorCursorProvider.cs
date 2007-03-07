using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Desktop;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// This class implements a simple algorithm for determining which cursor on an 8 point compass to show
	/// for a particular control point in a <see cref="ControlPointGroup"/>.  The class assumes that the
	/// purpose of the control points is to stretch the graphic that owns the control points (as will be
	/// the case with most ROI graphic implementations).
	/// </summary>
	public class CompassStretchIndicatorCursorProvider : ICursorTokenProvider
	{
		public enum CompassPoints { NorthEast = 0, SouthEast = 1, SouthWest = 2, NorthWest = 3, North = 4, East = 5, South = 6, West = 7 };

		private SortedList<CompassPoints, CursorToken> _stretchIndicatorTokens;

		private ControlPointGroup _controlPoints;

		public CompassStretchIndicatorCursorProvider(ControlPointGroup controlPoints)
			: this(controlPoints, true)
		{
		}

		public CompassStretchIndicatorCursorProvider(ControlPointGroup controlPoints, bool installDefaults)
		{
			Platform.CheckForNullReference(controlPoints, "controlPoints");

			_controlPoints = controlPoints;
			_stretchIndicatorTokens = new SortedList<CompassPoints, CursorToken>();
			if (installDefaults)
				InstallDefaults();
		}

		private CompassStretchIndicatorCursorProvider()
		{
		}

		/// <summary>
		/// Gets the bounding rectangle that contains all the points in the <see cref="ControlPointsGroup"/>.
		/// </summary>
		protected RectangleF BoundingRectangle
		{
			get
			{
				List<PointF> controlPoints = new List<PointF>();
				for (int i = 0; i < _controlPoints.Count; ++i)
					controlPoints.Add(_controlPoints[i]);

				return RectangleUtilities.ComputeBoundingRectangle(controlPoints.ToArray());
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that corresponds to a particular point on the compass.
		/// </summary>
		/// <param name="compassPoint">the compass point.</param>
		/// <returns>the <see cref="CursorToken"/> that corresponds to the specified point on the compass, or null.</returns>
		public CursorToken this[CompassPoints compassPoint]
		{
			get
			{
				if (!_stretchIndicatorTokens.ContainsKey(compassPoint))
					return null;

				return _stretchIndicatorTokens[compassPoint]; 
			}
			set
			{
				if (value != null)
				{
					_stretchIndicatorTokens[compassPoint] = value;
				}
				else
				{
					if (_stretchIndicatorTokens.ContainsKey(compassPoint))
						_stretchIndicatorTokens.Remove(compassPoint);
				}
			}
		}

		/// <summary>
		/// Installs the default set of system cursors for the compass.
		/// </summary>
		protected virtual void InstallDefaults()
		{
			_stretchIndicatorTokens[CompassPoints.East] =
				_stretchIndicatorTokens[CompassPoints.West] = new CursorToken(CursorToken.SystemCursors.SizeWE);

			_stretchIndicatorTokens[CompassPoints.North] =
				_stretchIndicatorTokens[CompassPoints.South] = new CursorToken(CursorToken.SystemCursors.SizeNS);

			_stretchIndicatorTokens[CompassPoints.NorthEast] =
				_stretchIndicatorTokens[CompassPoints.SouthWest] = new CursorToken(CursorToken.SystemCursors.SizeNESW);

			_stretchIndicatorTokens[CompassPoints.NorthWest] =
				_stretchIndicatorTokens[CompassPoints.SouthEast] = new CursorToken(CursorToken.SystemCursors.SizeNWSE);
		}

		/// <summary>
		/// Computes the distance from a point to a compass point on the given rectangle.
		/// </summary>
		/// <param name="point">a point whose distance from a compass point on the rectangle is to be determined.</param>
		/// <param name="compassRectangle">the rectangle from which to determine the compass point position.</param>
		/// <param name="compassPoint">the point on the compass to find the distance to.</param>
		/// <returns></returns>
		protected float DistanceToCompassPoint(PointF point, RectangleF compassRectangle, CompassPoints compassPoint)
		{
			PointF compassPointPosition = GetCompassPointPosition(compassPoint, compassRectangle);
			return (float)Vector.Distance(point, compassPointPosition);
		}

		/// <summary>
		/// Computes the position on a given rectangle that corresponds to the given compass point.
		/// </summary>
		/// <param name="compassPoint">the compass point whose position on the rectangle is to be determined.</param>
		/// <param name="rectangle">the rectangle.</param>
		/// <returns>the point on the rectangle that corresponds to the given compass point.</returns>
		protected PointF GetCompassPointPosition(CompassPoints compassPoint, RectangleF rectangle)
		{
			float top = rectangle.Top;
			float left = rectangle.Left;
			float right = rectangle.Right;
			float bottom = rectangle.Bottom;
			
			float centreX = left;
			if (left != right)
				centreX = rectangle.Left + rectangle.Width / 2F;

			float centreY = top;
			if (top != bottom)
				centreY = rectangle.Top + rectangle.Height / 2F;

			switch (compassPoint)
			{
				case CompassPoints.NorthWest:
					return new PointF(left, top);
				case CompassPoints.NorthEast:
					return new PointF(right, top);
				case CompassPoints.SouthEast:
					return new PointF(right, bottom);
				case CompassPoints.SouthWest:
					return new PointF(left, bottom);
				case CompassPoints.North:
					return new PointF(centreX, top);
				case CompassPoints.East:
					return new PointF(right, centreY);
				case CompassPoints.South:
					return new PointF(centreX, bottom);
				default: //CompassPoints.West:
					return new PointF(left, centreY);
			}
		}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the appropriate <see cref="CursorToken"/> for a given point (in destination coordinates).
		/// </summary>
		/// <param name="point">the point (in destination coordinates).</param>
		/// <returns>a <see cref="CursorToken"/> that is appropriate for the given point, or null.</returns>
		public virtual CursorToken GetCursorToken(Point point)
		{
			if (_stretchIndicatorTokens.Count == 0)
				return null;

			int controlPointIndex = _controlPoints.HitTestControlPoint(point);
			if (controlPointIndex < 0)
				return null;

			_controlPoints.CoordinateSystem = CoordinateSystem.Destination;

			PointF controlPoint = _controlPoints[controlPointIndex];
			RectangleF containingRectangle = this.BoundingRectangle;

			CompassPoints closestCompassPoint = _stretchIndicatorTokens.Keys[0];
			float minDistance = DistanceToCompassPoint(controlPoint, containingRectangle, closestCompassPoint);

			for (int i = 1; i < _stretchIndicatorTokens.Keys.Count; ++i)
			{
				CompassPoints compassPoint = _stretchIndicatorTokens.Keys[i];
				float distance = DistanceToCompassPoint(controlPoint, containingRectangle, compassPoint);

				if (distance <= minDistance)
				{
					closestCompassPoint = compassPoint;
					minDistance = distance;
				}
			}

			_controlPoints.ResetCoordinateSystem();
		
			return _stretchIndicatorTokens[closestCompassPoint];
		}

		#endregion
	}
}
