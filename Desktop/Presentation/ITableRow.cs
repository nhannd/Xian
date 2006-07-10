using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public interface ITableRow
    {
        object Item { get; }
        object GetValue(int column);
    }
}
