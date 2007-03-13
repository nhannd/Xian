using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines a column in an <see cref="ITable"/>
    /// </summary>
    public interface ITableColumn
    {
        /// <summary>
        /// The name or heading of the column
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of data that the column holds
        /// </summary>
        Type ColumnType { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this column is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		event EventHandler VisibilityChanged;

        /// <summary>
        /// A factor that influences the width of the column relative to other columns.
        /// A value of 1.0 is default.
        /// </summary>
        float WidthFactor { get; }

        /// <summary>
        /// Gets the width of this column as a percentage of the overall table width.
        /// </summary>
        int WidthPercent { get; }

        /// <summary>
        /// Indicates whether this column is read-only
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Gets the value of this column for the specified item
        /// </summary>
        /// <param name="item">The item from which the value is to be obtained</param>
        /// <returns>The value</returns>
        object GetValue(object item);

        /// <summary>
        /// Sets the value of this column on the specified item, assuming this is not a read-only column
        /// </summary>
        /// <param name="item">The item on which the value is to be set</param>
        /// <param name="value">The value</param>
        void SetValue(object item, object value);

        /// <summary>
        /// Get a comparer that can be used to sort items in the specified direction
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        IComparer GetComparer(bool ascending);
    }
}
