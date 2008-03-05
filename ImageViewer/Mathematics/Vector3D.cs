using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A simple 3D vector class.
	/// </summary>
	/// <remarks>
	/// The Vector3D class is immutable.  All necessary operations
	/// can be done via the operator overloads.
	/// </remarks>
	public class Vector3D : IEquatable<Vector3D>
	{
		private readonly float _x;
		private readonly float _y;
		private readonly float _z;

		/// <summary>
		/// Represents the empty (zero) vector.
		/// </summary>
		public static readonly Vector3D Empty = new Vector3D(0F, 0F, 0F);

		/// <summary>
		/// Constructor.
		/// </summary>
		public Vector3D(float x, float y, float z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		/// <summary>
		/// Gets or sets the x-component.
		/// </summary>
		public float X
		{
			get { return _x; }
		}

		/// <summary>
		/// Gets or sets the y-component.
		/// </summary>
		public float Y
		{
			get { return _y; }
		}

		/// <summary>
		/// Gets or sets the z-component.
		/// </summary>
		public float Z
		{
			get { return _z; }
		}

		/// <summary>
		/// Gets whether or not this is a 'null' vector (all components are zero).
		/// </summary>
		public bool IsNull
		{
			get { return _x == 0F && _y == 0F && _z == 0F; }	
		}

		/// <summary>
		/// Gets the magnitude of this vector.
		/// </summary>
		public float Magnitude
		{
			get
			{
				return (float)Math.Sqrt(_x*_x + _y*_y + _z*_z);
			}	
		}

		/// <summary>
		/// Clones the vector.
		/// </summary>
		public Vector3D Clone()
		{
			return new Vector3D(_x, _y, _z);
		}

		/// <summary>
		/// Normalizes the vector, or makes it a unit vector.
		/// </summary>
		public Vector3D Normalize()
		{
			return this / Magnitude;
		}

		/// <summary>
		/// Gets the dot-product of of this vector and <paramref name="right"/>.
		/// </summary>
		public float Dot(Vector3D right)
		{
			return _x * right.X + _y * right.Y + _z * right.Z;
		}

		/// <summary>
		/// Returns the cross-product of this vector and <paramref name="right"/>.
		/// </summary>
		public Vector3D Cross(Vector3D right)
		{
			Vector3D cross = new Vector3D(0, 0, 0);

			float x = _y * right.Z - _z * right.Y;
			float y = -_x * right.Z + _z * right.X;
			float z = _x * right.Y - _y * right.X;

			return new Vector3D(x, y, z);
		}

		/// <summary>
		/// Finds the intersection of the line segment defined by <paramref name="lineStart"/> and
		/// <paramref name="lineEnd"/> with a plane described by it's normal (<paramref name="planeNormal"/>)
		/// and an arbitrary point in the plane (<paramref name="pointInPlane/>).
		/// </summary>
		/// <param name="planeNormal">The normal vector of an arbitrary plane.</param>
		/// <param name="pointInPlane">A point in space that lies on the plane whose normal is <paramref name="planeNormal"/>.</param>
		/// <param name="lineStart">The position vector of the start of the line.</param>
		/// <param name="lineEnd">The position vector of the end of the line.</param>
		/// <param name="restrictToLineSegmentBoundaries">Specifies whether to restrict the point of intersection
		/// to a point on the line segment, or allow the line to be extended beyond it's boundaries.
		/// <returns>A position vector describing the point of intersection of the line with the plane, or null if the
		/// line and plane do not intersect.</returns>
		public static Vector3D GetIntersectionOfLineSegmentWithPlane(
			Vector3D planeNormal,
			Vector3D pointInPlane,
			Vector3D lineStart, 
			Vector3D lineEnd, 
			bool restrictToLineSegmentBoundaries)
		{
			if (Vector3D.AreEqual(planeNormal, Vector3D.Empty))
				return null;

			Vector3D line = lineEnd - lineStart;
			Vector3D planeToLineStart = pointInPlane - lineStart;

			float lineDotPlaneNormal = planeNormal.Dot(line);

			if (FloatComparer.AreEqual(0F, lineDotPlaneNormal))
				return null;

			float ratio = planeNormal.Dot(planeToLineStart) / lineDotPlaneNormal;

			if (restrictToLineSegmentBoundaries && (ratio < 0F || ratio > 1F))
				return null;

			return lineStart + ratio * line;
		}

		/// <summary>
		/// Returns a descriptive string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"({0:F8}, {1:F8}, {2:F8})", _x, _y, _z);
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of <paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator *(float scale, Vector3D vector)
		{
			return vector*scale;
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of <paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator *(Vector3D vector, float scale)
		{
			return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of 1/<paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator /(float scale, Vector3D vector)
		{
			return vector/scale;
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of 1/<paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator /(Vector3D vector, float scale)
		{
			return new Vector3D(vector.X / scale, vector.Y / scale, vector.Z / scale);
		}

		/// <summary>
		/// Adds <paramref name="left"/> and <paramref name="right"/> together.
		/// </summary>
		public static Vector3D operator +(Vector3D left, Vector3D right)
		{
			return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		/// <summary>
		/// Subtracts <paramref name="right"/> from <paramref name="left"/>.
		/// </summary>
		public static Vector3D operator -(Vector3D left, Vector3D right)
		{
			return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a given tolerance (per vector component).
		/// </summary>
		public static bool AreEqual(Vector3D left, Vector3D right, float tolerance)
		{
			return FloatComparer.AreEqual(left.X, right.X, tolerance) &&
					FloatComparer.AreEqual(left.Y, right.Y, tolerance) &&
					FloatComparer.AreEqual(left.Z, right.Z, tolerance);
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a small tolerance (per vector component).
		/// </summary>
		public static bool AreEqual(Vector3D left, Vector3D right)
		{
			return FloatComparer.AreEqual(left.X, right.X) &&
					FloatComparer.AreEqual(left.Y, right.Y) && 
					FloatComparer.AreEqual(left.Z, right.Z);
		}

		/// <summary>
		/// Gets a hash code for the vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (int)(3 * _x.GetHashCode() + 5 * _y.GetHashCode() + 7 * _z.GetHashCode());
		}

		/// <summary>
		/// Gets whether or not this object equals <paramref name="obj"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as Vector3D);
		}

		#region IEquatable<Vector3D> Members

		/// <summary>
		/// Gets whether or not this object equals <paramref name="other"/>.
		/// </summary>
		public bool Equals(Vector3D other)
		{
			if (other == null)
				return false;

			return (X == other.X && Y == other.Y && Z == other.Z);
		}

		#endregion
	}
}
