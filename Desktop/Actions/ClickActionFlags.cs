using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Set of flags that customize the behaviour of click actions.
    /// </summary>
    [Flags]
    public enum ClickActionFlags
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Specifies that the action is a "check" action (e.g. that it has toggle behaviour).
        /// </summary>
        CheckAction = 0x01
    }
}
