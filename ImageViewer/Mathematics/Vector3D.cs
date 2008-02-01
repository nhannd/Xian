using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Mathematics
{
	// TODO: this class should be unit tested.

	/// <summary>
	/// A simple 3D vector class.
	/// </summary>
	public class Vector3D : IEquatable<Vector3D>
	{
		private float _x;
		private float _y;
		private float _z;

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
			set { _x = value; }
		}

		/// <summary>
		/// Gets or sets the y-component.
		/// </summary>
		public float Y
		{
			get { return _y; }
			set { _y = value; }
		}

		/// <summary>
		/// Gets or sets the z-component.
		/// </summary>
		public float Z
		{
			get { return _z; }
			set { _z = value; }
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

			cross.X = _y * right.Z - _z * right.Y;
			cross.Y = -_x * right.Z + _z * right.X;
			cross.Z = _x * right.Y + _y * right.X;

			return cross;
		}

		/// <summary>
		/// Returns a descriptive string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"({0:F8}, {1:F8}, {2:F8})", _x, _y, _z);
		}

		/// <summary>
		/// Gets a null vector, where all components are zero.
		/// </summary>
		public static Vector3D GetNullVector()
		{
			return new Vector3D(0F, 0F, 0F);
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

		public override int GetHashCode()
		{
			return (int)(3 * _x.GetHashCode() + 5 * _y.GetHashCode() + 7 * _z.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as Vector3D);
		}

		#region IEquatable<Vector3D> Members

		public bool Equals(Vector3D other)
		{
			if (other == null)
				return false;

			return (X == other.X && Y == other.Y && Z == other.Z);
		}

		#endregion
	}
}
