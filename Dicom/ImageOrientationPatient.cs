using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// The ImageOrientationPatient class is quite simple, basically providing a centralized place to store the
	/// row/column direction cosines from the Dicom Header.  One additional piece of functionality is the primary
	/// and secondary row/column directions, which are transformed (using the cosines) into more meaningful 
	/// values (Anterior, Left, Head, etc).
	/// 
	/// The components of each of the cosine vectors (row/column, x,y,z) corresponds to the patient based coordinate
	/// system as follows (e.g. it is a right-handed system): 
	///
	/// +x --> Left,			-x --> Right
	/// +y --> Posterior,		-y --> Anterior 
	/// +z --> Head,			-z --> Foot
	/// 
	/// The primary & secondary directions of a cosine vector correspond directly to the 2 largest
	/// values in the cosine vector, disregarding the sign.  The sign determines the direction along 
	/// a particular axis in the patient based system as described above.
	///
	/// The row cosine vector completely describes the direction, in the patient based system, of the first row
	/// in the image (increasing x).  Similarly, the column cosine vector completely describes the 
	/// direction of the first column in the image in the patient based system.
	/// </summary>
	public class ImageOrientationPatient
	{
		public enum Directions { None = 0, Left = 1, Right = -1, Posterior = 2, Anterior = -2, Head = 3, Foot = -3 };

		#region Private Members

		private double _rowX;
		private double _rowY;
		private double _rowZ;
		private double _columnX;
		private double _columnY;
		private double _columnZ;

		private int _primaryRowDirection;
		private int _secondaryRowDirection;
		private int _primaryColumnDirection;
		private int _secondaryColumnDirection;

		#endregion

		public ImageOrientationPatient(double rowX, double rowY, double rowZ, double columnX, double columnY, double columnZ)
		{
			_rowX = rowX;
			_rowY = rowY;
			_rowZ = rowZ;
			_columnX = columnX;
			_columnY = columnY;
			_columnZ = columnZ;

			Recalculate();
		}

		protected ImageOrientationPatient()
		{ 
			_rowX = _rowY = _rowZ = _columnX = _columnY = _columnZ = 0;
			Recalculate();
		}

		public double RowX
		{
			get { return _rowX; }
			set
			{
				if (_rowX != value)
				{
					_rowX = value;
					Recalculate();
				}
			}
		}

		public double RowY
		{
			get { return _rowY; }
			set
			{
				if (_rowY != value)
				{
					_rowY = value;
					Recalculate();
				}
			}
		}

		public double RowZ
		{
			get { return _rowZ; }
			set
			{
				if (_rowZ != value)
				{
					_rowZ = value;
					Recalculate();
				}
			}
		}

		public double ColumnX
		{
			get { return _columnX; }
			set
			{
				if (_columnX != value)
				{
					_columnX = value;
					Recalculate();
				}
			}
		}

		public double ColumnY
		{
			get { return _columnY; }
			set
			{
				if (_columnY != value)
				{
					_columnY = value;
					Recalculate();
				}
			}
		}

		public double ColumnZ
		{
			get { return _columnZ; }
			set
			{
				if (_columnZ != value)
				{
					_columnZ = value;
					Recalculate();
				}
			}
		}

		/// <summary>
		/// Gets the primary direction, in terms of the Patient based coordinate system, of the first row of the Image (increasing x).
		/// </summary>
		/// <param name="opposingDirection">indicates the opposite direction to the primary direction should be returned.
		/// For example, if the primary direction is Anterior, then Posterior will be returned if this parameter is true.</param>
		/// <returns>the direction, in terms of the Patient based coordinate system</returns>
		public Directions GetPrimaryRowDirection(bool opposingDirection)
		{
			return (Directions)(_primaryRowDirection * (opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Gets the primary direction, in terms of the Patient based coordinate system, of the first column of the Image (increasing y).
		/// </summary>
		/// <param name="opposingDirection">indicates the opposite direction to the primary direction should be returned.
		/// For example, if the primary direction is Anterior, then Posterior will be returned if this parameter is true.</param>
		/// <returns>the direction, in terms of the Patient based coordinate system</returns>
		public Directions GetPrimaryColumnDirection(bool opposingDirection)
		{
			return (Directions)(_primaryColumnDirection * (opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Gets the secondary direction, in terms of the Patient based coordinate system, of the first row of the Image (increasing x).
		/// </summary>
		/// <param name="opposingDirection">indicates the opposite direction to the secondary direction should be returned.
		/// For example, if the secondary direction is Anterior, then Posterior will be returned if this parameter is true.</param>
		/// <returns>the direction, in terms of the Patient based coordinate system</returns>

		public Directions GetSecondaryRowDirection(bool opposingDirection)
		{
			return (Directions)(_secondaryRowDirection * (opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Gets the secondary direction, in terms of the Patient based coordinate system, of the first column of the Image (increasing y).
		/// </summary>
		/// <param name="opposingDirection">indicates the opposite direction to the secondary direction should be returned.
		/// For example, if the secondary direction is Anterior, then Posterior will be returned if this parameter is true.</param>
		/// <returns>the direction, in terms of the Patient based coordinate system</returns>
		public Directions GetSecondaryColumnDirection(bool opposingDirection)
		{
			return (Directions)(_secondaryColumnDirection * (opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Recalculates the primary/secondary directions (in patient based system) for the first row and first column.
		/// </summary>
		private void Recalculate()
		{
			double[] rowCosines = new double[] { _rowX, _rowY, _rowZ };
			double[] columnCosines = new double[] { _columnX, _columnY, _columnZ };

			int[] rowCosineSortedIndices = BubbleSortCosineIndices(rowCosines);
			int[] columnCosineSortedIndices = BubbleSortCosineIndices(columnCosines);

			SetDirectionValue(ref _primaryRowDirection, rowCosines[rowCosineSortedIndices[0]], rowCosineSortedIndices[0]);
			SetDirectionValue(ref _secondaryRowDirection, rowCosines[rowCosineSortedIndices[1]], rowCosineSortedIndices[1]);
			SetDirectionValue(ref _primaryColumnDirection, columnCosines[columnCosineSortedIndices[0]], columnCosineSortedIndices[0]);
			SetDirectionValue(ref _secondaryColumnDirection, columnCosines[columnCosineSortedIndices[1]], columnCosineSortedIndices[1]);
		}

		/// <summary>
		/// Sets one of the member primary/secondary direction variables.
		/// </summary>
		/// <param name="member">the member to set</param>
		/// <param name="cosineValue">the cosine value</param>
		/// <param name="cosineIndex">the index of the cosine value in the original direction cosine vector</param>
		private void SetDirectionValue(ref int member, double cosineValue, int cosineIndex)
		{
			member = 0;
			if (Math.Abs(cosineValue) > (double)float.Epsilon)
				member = (cosineIndex + 1) * Math.Sign(cosineValue);
		}

		/// <summary>
		/// Bubble sorts an array of cosines in descending order (largest to smallest), ignoring the sign.
		/// This helps to determine the primary/secondary directions for the cosines.
		/// </summary>
		/// <param name="cosineArray">the array of cosines (row or column)</param>
		/// <returns>an array of indices into the input array (cosineArray), that when applied would sort the cosines appropriately.</returns>
		private int[] BubbleSortCosineIndices(double[] cosineArray)
		{
			int[] indexArray = new int[] { 0, 1, 2 };

			for (int i = 2; i > 0; --i)
			{
				for (int j = 0; j < i; ++j)
				{
					if (Math.Abs(cosineArray[indexArray[j + 1]]) > Math.Abs(cosineArray[indexArray[j]]))
					{
						int tempint = indexArray[j];
						indexArray[j] = indexArray[j + 1];
						indexArray[j + 1] = tempint;
					}
				}
			}

			return indexArray;
		}
	}
}
