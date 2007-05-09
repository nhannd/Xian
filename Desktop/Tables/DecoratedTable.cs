using System;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tables
{
    public class DecoratedTable<TItem> :  Table<TItem>, IDecoratedTable
    {
        private uint _cellRowCount;
        private ColorSelector _backgroundColorSelector;
        private ColorSelector _outlineColorSelector;

        public DecoratedTable(uint cellRowCount)
            : base()
        {
            Platform.CheckArgumentRange((int)cellRowCount, 1, int.MaxValue, "cellRowCount");
            _cellRowCount = cellRowCount;
        }

        #region IDecoratedTable Members

        public uint CellRowCount
        {
            get { return _cellRowCount; }
        }
        
        public ColorSelector BackgroundColorSelector
        {
            get { return _backgroundColorSelector; }
            set { _backgroundColorSelector = value; }
        }

        public ColorSelector OutlineColorSelector
        {
            get { return _outlineColorSelector; }
            set { _outlineColorSelector = value; }
        }

        #endregion
    }
}
