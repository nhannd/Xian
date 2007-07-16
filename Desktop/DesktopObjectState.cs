using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the possible states for a <see cref="DesktopObject"/>
    /// </summary>
    public enum DesktopObjectState
    {
        New,
        Opening,
        Open,
        Closing,
        Closed
    }
}
