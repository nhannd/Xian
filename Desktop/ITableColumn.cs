using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Used by the <see cref="TableData"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item on which the table is based</typeparam>
    public interface ITableColumn<TItem>
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
        /// A factor that influences the width of the column relative to other columns.
        /// A value of 1.0 is default.
        /// </summary>
        float WidthFactor { get; }

        /// <summary>
        /// Indicates whether this column is read-only
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Gets the value of this column for the specified item
        /// </summary>
        /// <param name="item">The item from which the value is to be obtained</param>
        /// <returns>The value</returns>
        object GetValue(TItem item);

        /// <summary>
        /// Sets the value of this column on the specified item, assuming this is not a read-only column
        /// </summary>
        /// <param name="item">The item on which the value is to be set</param>
        /// <param name="value">The value</param>
        void SetValue(TItem item, object value);
    }
}
