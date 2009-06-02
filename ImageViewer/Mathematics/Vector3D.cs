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
		private float _x;
		private float _y;
		private float _z;

		/// <summary>
		/// Represents the Zero vector.
		/// </summary>
		public static readonly Vector3D Null = new Vector3D(0F, 0F, 0F);

		/// <summary>
		/// Represents the unit vector in the direction of the positive X axis.
		/// </summary>
		public static readonly Vector3D xUnit = new Vector3D(1F, 0F, 0F);

		/// <summary>
		/// Represents the unit vector in the direction of the positive Y axis.
		/// </summary>
		public static readonly Vector3D yUnit = new Vector3D(0F, 1F, 0F);

		/// <summary>
		/// Represents the unit vector in the direction of the positive Z axis.
		/// </summary>
		public static readonly Vector3D zUnit = new Vector3D(0F, 0F, 1F);

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
		/// Copy Constructor.
		/// </summary>
		public Vector3D(Vector3D src)
		{
			_x = src.X;
			_y = src.Y;
			_z = src.Z;
		}

		/// <summary>
		/// Gets the x-component.
		/// </summary>
		public float X
		{
			get { return _x; }
		}

		/// <summary>
		/// Gets the y-component.
		/// </summary>
		public float Y
		{
			get { return _y; }
		}

		/// <summary>
		/// Gets the z-component.
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
				return (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
			}
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
			float x = _y * right.Z - _z * right.Y;
			float y = -_x * right.Z + _z * right.X;
			float z = _x * right.Y - _y * right.X;

			return new Vector3D(x, y, z);
		}

		/// <summary>
		/// Determines whether or not this vector is parallel to <paramref name="other"/> within a certain <paramref name="angleTolerance"/>.
		/// </summary>
		public bool IsParallel(Vector3D other, float angleTolerance)
		{
			angleTolerance = Math.Abs(angleTolerance);
			float upper = angleTolerance;
			float lower = -angleTolerance;

			float angle = GetAngleBetween(other);

			bool parallel = FloatComparer.IsGreaterThan(angle, lower) && FloatComparer.IsLessThan(angle, upper);

			if (!parallel)
			{
				upper = (float)Math.PI + angleTolerance;
				lower = (float)Math.PI - angleTolerance;
				parallel = FloatComparer.IsGreaterThan(angle, lower) && FloatComparer.IsLessThan(angle, upper);
			}

			return parallel;
		}

		/// <summary>
		/// Determines whether or not this vector is orthogonal to <paramref name="other"/> within a certain <paramref name="angleTolerance"/>.
		/// </summary>
		public bool IsOrthogonal(Vector3D other, float angleTolerance)
		{
			angleTolerance = Math.Abs(angleTolerance);
			float upper = (float)Math.PI / 2 + angleTolerance;
			float lower = (float)Math.PI / 2 - angleTolerance;

			float angle = GetAngleBetween(other);

			return FloatComparer.IsGreaterThan(angle, lower) && FloatComparer.IsLessThan(angle, upper);
		}

		/// <summary>
		/// Gets the angle between this vector and <paramref name="other"/> in radians.
		/// </summary>
		public float GetAngleBetween(Vector3D other)
		{
			Vector3D normal1 = this.Normalize();
			Vector3D normal2 = other.Normalize();

			// the vectors are already normalized, so we don't need to divide by the magnitudes.
			float dot = normal1.Dot(normal2);

			if (dot < -1F)
				dot = -1F;
			if (dot > 1F)
				dot = 1F;

			return Math.Abs((float)Math.Acos(dot));
		}

		/// <summary>
		/// Finds the intersection of the line segment defined by <paramref name="linePoint1"/> and
		/// <paramref name="linePoint2"/> with a plane described by it's normal (<paramref name="planeNormal"/>)
		/// and an arbitrary point in the plane (<paramref name="pointInPlane"/>).
		/// </summary>
		/// <param name="planeNormal">The normal vector of an arbitrary plane.</param>
		/// <param name="pointInPlane">A point in space that lies on the plane whose normal is <paramref name="planeNormal"/>.</param>
		/// <param name="linePoint1">The position vector of the start of the line.</param>
		/// <param name="linePoint2">The position vector of the end of the line.</param>
		/// <param name="isLineSegment">Specifies whether <paramref name="linePoint1"/> and <paramref name="linePoint2"/>
		/// define a line segment, or simply 2 points on an infinite line.</param>
		/// <returns>A position vector describing the point of intersection of the line with the plane, or null if the
		/// line and plane do not intersect.</returns>
		public static Vector3D GetLinePlaneIntersection(
			Vector3D planeNormal,
			Vector3D pointInPlane,
			Vector3D linePoint1,
			Vector3D linePoint2,
			bool isLineSegment)
		{
			if (Vector3D.AreEqual(planeNormal, Vector3D.Null))
				return null;

			Vector3D line = linePoint2 - linePoint1;
			Vector3D planeToLineStart = pointInPlane - linePoint1;

			float lineDotPlaneNormal = planeNormal.Dot(line);

			if (FloatComparer.AreEqual(0F, lineDotPlaneNormal))
				return null;

			float ratio = planeNormal.Dot(planeToLineStart) / lineDotPlaneNormal;

			if (isLineSegment && (ratio < 0F || ratio > 1F))
				return null;

			return linePoint1 + ratio * line;
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
			return vector * scale;
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
			return vector / scale;
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
		/// Returns the negative of the given vector.
		/// </summary>
		public static Vector3D operator -(Vector3D vector)
		{
			return -1 * vector;
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
			return 3 * _x.GetHashCode() + 5 * _y.GetHashCode() + 7 * _z.GetHashCode();
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
