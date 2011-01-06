#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {

        protected TimeSpan MinCountDownDuration
        {
            get
            {
                double duration = 30; // seconds
                double.TryParse(ConfigurationManager.AppSettings.Get("ClientTimeoutWarningMinDuration"), out duration);
                duration = Math.Min(duration, (SessionManager.Current.Credentials.SessionToken.ExpiryTime - Platform.Time).TotalSeconds);

                return TimeSpan.FromSeconds(duration);
            }
        }
  }

}