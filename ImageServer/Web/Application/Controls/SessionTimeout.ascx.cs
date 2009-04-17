using System;
using System.Data;
using System.Configuration;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {

        protected TimeSpan MinCountDownDuration
        {
            get
            {
                int duration = 30; // seconds
                Int32.TryParse(ConfigurationManager.AppSettings.Get("ClientTimeoutWarningMinDuration"), out duration);
                duration = Math.Min(duration, (int) SessionManager.SessionTimeout.TotalSeconds);
                return TimeSpan.FromSeconds(duration);
            }
        }
  }

}