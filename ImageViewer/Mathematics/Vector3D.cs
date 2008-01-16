using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Mathematics
{
	// TODO: this class should be unit tested.

	/// <summary>
	/// A simple 3D vector class.
	/// </summary>
	public class Vector3D
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
		/// <remarks>
		/// This method modifies the internal components of the object.
		/// </remarks>
		public void Normalize()
		{
			float magnitude = Magnitude;

			_x /= magnitude;
			_y /= magnitude;
			_z /= magnitude;
		}

		/// <summary>
		/// Multiplies this vector by a scale factor.
		/// </summary>
		/// <remarks>
		/// This method modifies the internal components of the object.
		/// </remarks>
		public void Multiply(float scale)
		{
			_x *= scale;
			_y *= scale;
			_z *= scale;
		}

		/// <summary>
		/// Adds another vector to this one.
		/// </summary>
		/// <remarks>
		/// This method modifies the internal components of the class.
		/// </remarks>
		public void Add(Vector3D right)
		{
			_x += right.X;
			_y += right.Y;
			_z += right.Z;
		}

		/// <summary>
		/// Subtracts another vector from this one.
		/// </summary>
		/// <remarks>
		/// This method modifies the internal components of the class.
		/// </remarks>
		public void Subtract(Vector3D right)
		{
			_x -= right.X;
			_y -= right.Y;
			_z -= right.Z;
		}

		/// <summary>
		/// Returns a descriptive string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"({0:F8}, {1:F8}, {2:F8})", _x, _y, _z);
		}

		/// <summary>
		/// Gets the dot product of two vectors.
		/// </summary>
		public static float Dot(Vector3D left, Vector3D right)
		{
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		/// <summary>
		/// Computes and returns a vector that is the cross product of the input vectors.
		/// </summary>
		public static Vector3D Cross(Vector3D left, Vector3D right)
		{
			Vector3D cross = new Vector3D(0, 0, 0);

			cross.X = left.Y * right.Z - left.Z * right.Y;
			cross.Y = -left.X * right.Z + left.Z * right.X;
			cross.Z = left.X * right.Y + left.Y * right.X;
			
			return cross;
		}

		/// <summary>
		/// Gets a null vector, where all components are zero.
		/// </summary>
		public static Vector3D GetNullVector()
		{
			return new Vector3D(0F, 0F, 0F);
		}
	}
}
