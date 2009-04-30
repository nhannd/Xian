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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Mathematics
{
	// TODO: Determinant, Inverse, etc are still missing.

	/// <summary>
	/// A simple matrix class.
	/// </summary>
	public class Matrix
	{
		private readonly int _rows;
		private readonly int _columns;

		private readonly float[,] _matrix;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Matrix(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			_rows = rows;
			_columns = columns;

			_matrix = new float[rows, columns];
		}

		public Matrix(int rows, int columns, float[,] matrix)
		{
			_matrix = matrix;
			_rows = rows;
			_columns = columns;
		}

		public Matrix(Matrix src)
		{
			_rows = src._rows;
			_columns = src._columns;
			_matrix = (float[,])src._matrix.Clone();
		}

		/// <summary>
		/// Gets the number of rows in the matrix.
		/// </summary>
		public int Rows
		{
			get { return _rows; }	
		}

		/// <summary>
		/// Gets the number of columns in the matrix.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets whether or not the matrix is square (rows == columns).
		/// </summary>
		public bool IsSquare
		{
			get { return _rows == _columns; }
		}

		/// <summary>
		/// Gets whether or not this is an identity matrix.
		/// </summary>
		public bool IsIdentity
		{
			get
			{
				if (!IsSquare)
					return false;

				for(int row = 0; row < _rows; ++row)
				{
					for(int column = 0; column < _columns; ++column)
					{
						if (_matrix[row, column] != ((row == column) ? 1F : 0F))
							return false;
					}
				}

				return true;
			}	
		}

		/// <summary>
		/// Gets the value of the matrix at the specified row and column indices.
		/// </summary>
		public float this[int row, int column]
		{
			get
			{
				Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, _columns - 1, "column");

				return _matrix[row, column];
			}
			set
			{
				Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, _columns - 1, "column");

				_matrix[row, column] = value;
			}
		}
		
		private void Scale(float scale)
		{
			for (int row = 0; row < _rows; ++row)
			{
				for (int column = 0; column < _columns; ++column)
					this[row, column] = this[row, column] * scale;
			}
		}

		/// <summary>
		/// Sets all the values in a particular row.
		/// </summary>
		/// <remarks>
		/// This is more efficient than setting each value separately.
		/// </remarks>
		public void SetRow(int row, params float[] values)
		{
			Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
			Platform.CheckTrue(values.Length == _columns, "number of parameters == _columns");

			for (int column = 0; column < _columns; ++column)
				_matrix[row, column] = values[column];
		}

		/// <summary>
		/// Sets all the values in a particular column.
		/// </summary>
		/// <remarks>
		/// This is more efficient than setting each value separately.
		/// </remarks>
		public void SetColumn(int column, params float[] values)
		{
			Platform.CheckArgumentRange(column, 0, _columns - 1, "column");
			Platform.CheckTrue(values.Length == _rows, "number of parameters == _rows");

			for (int row = 0; row < _rows; ++row)
				_matrix[row, column] = values[row];
		}
		
		/// <summary>
		/// Clones this matrix and its values.
		/// </summary>
		public Matrix Clone()
		{
			float[,] matrix = (float[,])_matrix.Clone();
			for (int row = 0; row < _rows; ++row)
			{
				for (int column = 0; column < _columns; ++column)
					matrix[row, column] = _matrix[row, column];
			}
			
			return new Matrix(_rows, _columns, matrix);
		}

		/// <summary>
		/// Returns a matrix that is the transpose of this matrix.
		/// </summary>
		public Matrix Transpose()
		{
			Matrix transpose = new Matrix(_columns, _rows);

			for (int row = 0; row < Rows; ++row)
			{
				for (int column = 0; column < Columns; ++column)
					transpose[column, row] = this[row, column];
			}

			return transpose;
		}

		/// <summary>
		/// Gets a string describing the matrix.
		/// </summary>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			for(int row = 0; row < _rows; ++row)
			{
				builder.Append("( ");
				for(int column = 0; column < _columns; ++column)
				{
					builder.Append(_matrix[row, column].ToString("F4"));
					if (column < (_columns - 1))
						builder.Append("  ");
				}

				builder.Append(")");
				if (row < (_rows - 1))
					builder.Append(", ");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets an identity matrix with the input dimensions.
		/// </summary>
		public static Matrix GetIdentity(int dimensions)
		{
			Matrix matrix = new Matrix(dimensions, dimensions);
			for (int i = 0; i < dimensions; ++i)
				matrix[i, i] = 1.0F;

			return matrix;
		}

		/// <summary>
		/// Multiplies <paramref name="left"/> and <paramref name="right"/> together.
		/// </summary>
		public static Matrix operator *(Matrix left, Matrix right)
		{
			if (left.Columns != right.Rows)
				throw new ArgumentException("Cannot multiply the two matrices together; their sizes are incompatible.");

			Matrix result = new Matrix(left.Rows, right.Columns);
			int mutualDimension = right.Rows;

			for (int row = 0; row < result.Rows; ++row)
			{
				for (int column = 0; column < result.Columns; ++column)
				{
					float value = 0F;

					for (int k = 0; k < mutualDimension; ++k)
						value = value + left[row, k] * right[k, column];

					result[row, column] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// Multiplies <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix operator *(float scale, Matrix matrix)
		{
			return matrix*scale;
		}

		/// <summary>
		/// Multiplies <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix operator *(Matrix matrix, float scale)
		{
			Matrix clone = matrix.Clone();
			clone.Scale(scale);
			return clone;
		}

		/// <summary>
		/// Multiplies <paramref name="matrix"/> by 1/<paramref name="scale"/>.
		/// </summary>
		public static Matrix operator /(float scale, Matrix matrix)
		{
			return matrix/scale;
		}

		/// <summary>
		/// Multiplies <paramref name="matrix"/> by 1/<paramref name="scale"/>.
		/// </summary>
		public static Matrix operator /(Matrix matrix, float scale)
		{
			Matrix clone = matrix.Clone();
			clone.Scale(1/scale);
			return clone;
		}

		/// <summary>
		/// Adds <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static Matrix operator +(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			Matrix clone = left.Clone();

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
					clone[row, column] += right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Subtracts <paramref name="right"/> from <paramref name="left"/>.
		/// </summary>
		public static Matrix operator -(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			Matrix clone = left.Clone();

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
					clone[row, column] -= right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a given tolerance (per matrix component).
		/// </summary>
		public static bool AreEqual(Matrix left, Matrix right, float tolerance)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
				{
					if (!FloatComparer.AreEqual(left[row, column], right[row, column], tolerance))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a small tolerance (per matrix component).
		/// </summary>
		public static bool AreEqual(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
				{
					if (!FloatComparer.AreEqual(left[row, column], right[row, column]))
						return false;
				}
			}

			return true;
		}
	}
}
