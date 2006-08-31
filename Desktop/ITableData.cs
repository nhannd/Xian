using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
#if !MONO
    /// <summary>
    /// Defines the interface for a table data source (data that is
    /// displayed on the screen in the form of a table)
    /// </summary>
    public interface ITableData : IBindingList, ITypedList
    {
    }
#endif
}
