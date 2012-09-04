#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using System.Diagnostics;
using System.Reflection;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("logOut", "global-menus/MenuLogout", "Logout")]
    [GroupHint("logOut", "Application.Logout")]
    [ExtensionOf(typeof (DesktopToolExtensionPoint), Enabled = true)]
    class LogoutTool: Tool<IDesktopToolContext>
    {
        public void Logout()
        {
            if (ClearCanvas.Desktop.Application.Quit())
            {
                Platform.Log(LogLevel.Info, @"User has logged out");
                Platform.Log(LogLevel.Info, "Restarting the application");
                Process.Start(Assembly.GetEntryAssembly().Location);
            }
        }
    }
}
