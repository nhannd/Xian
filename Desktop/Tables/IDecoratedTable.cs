using System;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tables
{
    public delegate string ColorSelector(object o);

    public interface IDecoratedTable
    {
        uint CellRowCount { get; }
        ColorSelector BackgroundColorSelector { get; set; }
        ColorSelector OutlineColorSelector { get; set; }
    }
}
