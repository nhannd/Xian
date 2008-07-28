using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Alert;

namespace ClearCanvas.ImageServer.Services.Common.Alert
{
    public interface IAlertServiceExtension
    {
        void OnAlert(AlertCategory category, AlertLevel level, String source, String message);
        void OnAlert(AlertCategory category, AlertLevel level, String source, String message, DateTime expirationTime);
    }

    public class AlertServiceExtensionPoint : ExtensionPoint<IAlertServiceExtension> { }
}
