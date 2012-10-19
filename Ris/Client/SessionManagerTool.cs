#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("changePassword", "global-menus/MenuTools/Change Password", "ChangePassword")]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class SessionManagerTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        public SessionManagerTool()
        {
        }

        public void ChangePassword()
        {
            if(SessionManager.ChangePassword())
            {
                this.Context.DesktopWindow.ShowMessageBox("Password changed.", MessageBoxActions.Ok);
            }
        }
    }
}
