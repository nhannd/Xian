using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Contains static methods used to show progress dialog box
    /// </summary>
    public static class ProgressDialog
    {
        /// <summary>
        /// Show the progress dialog to to the user
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog</param>
        public static void Show(BackgroundTask task, IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                new ProgressDialogComponent(task),
                Application.ApplicationName);
        }
    }
}
