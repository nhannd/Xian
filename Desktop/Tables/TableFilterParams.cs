namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Filter parameters that can be applied to a table
    /// </summary>
    public class TableFilterParams
    {
        private ITableColumn _column;
        private object value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_column">The column to filter by, null to filter by any column</param>
        /// <param name="value">The value to filter by</param>
        public TableFilterParams(ITableColumn _column, object value)
        {
            this._column = _column;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the column to filter by
        /// </summary>
        public ITableColumn Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// Gets or sets the value to filter by
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}