using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Mathematics
{
	// TODO: this class should be unit tested.

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
		
		/// <summary>
		/// Multiplies two matrices together and returns the result as another matrix.
		/// </summary>
		public Matrix Multiply(Matrix right)
		{
			Matrix result = new Matrix(_rows, right.Columns);
			Multiply(this, right, result);
			return result;
		}

		/// <summary>
		/// Returns a matrix that is the transpose of this matrix.
		/// </summary>
		public Matrix Transpose()
		{
			Matrix transpose = new Matrix(_columns, _rows);
			Transpose(this, transpose);
			return transpose;
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
		/// Transposes the input <paramref name="original"/> matrix, filling the <paramref name="result"/>.
		/// </summary>
		protected static void Transpose(Matrix original, Matrix result)
		{
			if (original.Rows != result.Columns || original.Columns != result.Rows)
				throw new ArgumentException("Transpose failed; the dimensions are invalid.");

			for (int row = 0; row < original.Rows; ++row)
			{
				for (int column = 0; column < original.Columns; ++column)
				{
					result[column, row] = original[row, column];
				}
			}
		}

		/// <summary>
		/// Multiplies <paramref name="left"/> and <paramref name="right"/>, putting the results in <paramref name="result"/>
		/// </summary>
		protected static void Multiply(Matrix left, Matrix right, Matrix result)
		{
			if (left.Columns != right.Rows || left.Rows != result.Rows || right.Columns != result.Columns)
				throw new ArgumentException("Cannot multiply the two matrices together; their sizes are incompatible.");

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
		}
	}
}
