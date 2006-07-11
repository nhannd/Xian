using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public interface ISelection
    {
        /// <summary>
        /// Returns the set of rows that are currently selected
        /// </summary>
        ITableRow[] SelectedRows { get; }

        /// <summary>
        /// Convenience method to obtain the currently selected row in a single-select scenario.
        /// If no rows are selected, the method returns null.  If more than one row is selected,
        /// it is undefined which row will be returned.
        /// </summary>
        ITableRow SelectedRow { get; }
    }
}
