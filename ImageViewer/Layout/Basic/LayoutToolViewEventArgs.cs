using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public class LayoutToolViewEventArgs : EventArgs
    {
        private int _imageBoxRows;
        private int _imageBoxColumns;
        private int _tileRows;
        private int _tileColumns;

        public LayoutToolViewEventArgs(int imageBoxRows, int imageBoxColumns, int tileRows, int tileColumns)
        {
            _imageBoxRows = imageBoxRows;
            _imageBoxColumns = imageBoxColumns;
            _tileRows = tileRows;
            _tileColumns = tileColumns;
        }

        public int ImageBoxRows
        {
            get { return _imageBoxRows; }
        }

        public int ImageBoxColumns
        {
            get { return _imageBoxColumns; }
        }

        public int TileRows
        {
            get { return _tileRows; }
        }

        public int TileColumns
        {
            get { return _tileColumns; }
        }
    }
}
