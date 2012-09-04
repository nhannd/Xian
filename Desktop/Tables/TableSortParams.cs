#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Sort parameters that can be applied to a table.
    /// </summary>
    public class TableSortParams
    {
    	/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="column">The column to sort by.</param>
        /// <param name="ascending">True if the items should be sorted in ascending orders.</param>
        public TableSortParams(ITableColumn column, bool ascending)
        {
            Column = column;
            Ascending = ascending;
        }

    	/// <summary>
    	/// Gets or sets the column to sort by.
    	/// </summary>
    	public ITableColumn Column { get; set; }

    	/// <summary>
    	/// Gets or sets whether the items should be sorted in ascending or descending order.
    	/// </summary>
    	public bool Ascending { get; set; }

		/// <summary>
		/// Gets a comparer representing this sort.
		/// </summary>
		/// <returns></returns>
		public IComparer GetComparer()
		{
			return Column.GetComparer(Ascending);
		}
    }
}
