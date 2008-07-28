using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Common.Alert
{
    public enum AlertCategory
    {
        /// <summary>
        /// System alert
        /// </summary>
        System,

        /// <summary>
        /// Application alert
        /// </summary>
        Application,

        /// <summary>
        /// Security alert
        /// </summary>
        Security,

        /// <summary>
        /// User alert
        /// </summary>
        User
    }

    public enum AlertLevel
    {
        /// <summary>
        /// Alerts carrying information
        /// </summary>
        Informational,

        /// <summary>
        /// Alerts carrying warning message
        /// </summary>
        Warning,

        /// <summary>
        /// Alerts carrying error message
        /// </summary>
        Error,

        /// <summary>
        /// Alerts carrying critical information message
        /// </summary>
        Critical
    }

    public interface IAlertService
    {
        void GenerateAlert(AlertCategory catogry, AlertLevel level, String source, String message, DateTime expirationTime);
        void GenerateAlert(AlertCategory catogry, AlertLevel level, String source, String message);
        void GenerateAlert(AlertCategory catogry, AlertLevel level, String source, object data);
    }

}
