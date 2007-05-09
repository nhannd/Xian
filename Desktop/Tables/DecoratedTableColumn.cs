using System;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tables
{
    public class DecoratedTableColumn<TItem, TColumn> : TableColumn<TItem, TColumn>, IDecoratedTableColumn
    {
        private uint _cellRow;

        public DecoratedTableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor, uint cellRow)
            : base(columnName, valueGetter, widthFactor)
        {
            _cellRow = cellRow;

            if (_cellRow > 0)
                this.Visible = false;
        }

        #region IDecoratedTableColumn

        public uint CellRow
        {
            get { return _cellRow; }
        }

        #endregion
    }
}
