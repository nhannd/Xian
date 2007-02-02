using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Copied from System.Windows.Forms.ProgressBarStyle so we don't need dependency from System.Windows.Forms.
    /// </summary>
    public enum ProgressBarStyle
    {
        Blocks = 0,
        Continuous = 1,
        Marquee = 2,
    }

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
            Show(task, false, ProgressBarStyle.Blocks, desktopWindow);
        }

        /// <summary>
        /// Show the progress dialog to to the user
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute</param>
        /// <param name="autoClose">Close the progress dialog after task completion</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog</param>
        public static void Show(BackgroundTask task, bool autoClose, IDesktopWindow desktopWindow)
        {
            Show(task, autoClose, ProgressBarStyle.Blocks, desktopWindow);
        }

        /// <summary>
        /// Show the progress dialog to to the user
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute</param>
        /// <param name="autoClose">Close the progress dialog after task completion</param>
        /// <param name="progressBarStyle">Show the progressbar using Marquee style</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog</param>
        public static void Show(BackgroundTask task, bool autoClose, ProgressBarStyle progressBarStyle, IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                new ProgressDialogComponent(task, autoClose, progressBarStyle),
                Application.ApplicationName);
        }
    }
}
