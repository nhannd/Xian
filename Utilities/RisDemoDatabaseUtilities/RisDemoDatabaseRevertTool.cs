#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    //[MenuAction("reset", "global-menus/Demo/Reset Demo Database...")]
    [Tooltip("reset", "Resets the Demo Database")]
    [IconSet("reset", IconScheme.Colour, "Icons.ResetDemoDatabaseMedium.png", "Icons.ResetDemoDatabaseMedium.png", "Icons.ResetDemoDatabaseLarge.png")]
    [ClickHandler("reset", "Reset")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class ResetDemoDatabaseTool : Tool<IDesktopToolContext>
    {
        public ResetDemoDatabaseTool()
        {
        }

        public void Reset()
        {
            if (Platform.ShowMessageBox("Proceeding will revert the database.  Continue?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                if (!File.Exists(Platform.InstallDir + @"\datastore\demo-fresh.sqb"))
                {
                    Platform.ShowMessageBox("Cannot reset database because demo-fresh.sqb is missing from the datastore directory.");
                    return;
                }
                else
                {
                    File.Copy(Platform.InstallDir + @"\datastore\demo-fresh.sqb", Platform.InstallDir + @"\datastore\ris.sqb", true);
                    Platform.ShowMessageBox("The demo database has been reset.");
                }
            }
        }
    }
}

