#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class ActivityMonitorManager
    {
        private static Workspace _workspace;

        public static void Show(IDesktopWindow desktopWindow)
        {
            if (_workspace != null)
            {
                _workspace.Activate();
                return;
            }

            if (!PermissionsHelper.IsInRole(AuthorityTokens.ActivityMonitor.View))
            {
                desktopWindow.ShowMessageBox(SR.WarningActivityMonitorPermission, MessageBoxActions.Ok);
                return;
            }

            var component = new ActivityMonitorComponent();
            _workspace = ApplicationComponent.LaunchAsWorkspace(desktopWindow, component, SR.TitleActivityMonitor);
            _workspace.Closed += ((sender, args) =>
            {
                _workspace = null;
            });
        }
    }
}
