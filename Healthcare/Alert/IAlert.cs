using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionPoint()]
    public class AlertExtensionPoint : ExtensionPoint<IAlert>
    {
    }

    public interface IAlert
    {
        IAlertNotification Test(object o);
    }
}
