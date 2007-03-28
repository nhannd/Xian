using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Alert
{
    public interface IAlertNotification
    {
        string Representation { get;}
        string Severity { get; }
        string Type { get; }
    }
}
