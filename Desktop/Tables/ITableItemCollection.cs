using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ClearCanvas.Desktop.Tables
{
    public interface ITableItemCollection : System.Collections.IEnumerable
    {
        object this[int index] { get; }
        int Count { get; }
    }
}
