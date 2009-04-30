#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Copied from System.Windows.Forms.ProgressBarStyle so we don't need dependency from System.Windows.Forms.
    /// </summary>
    public enum ProgressBarStyle
    {
        /// <summary>
        /// The progress bar appears as block segments.
        /// </summary>
		Blocks = 0,

		/// <summary>
		/// The progress bar is continuous, not blocky.
		/// </summary>
        Continuous = 1,

		/// <summary>
		/// Marquee mode, doesn't actually show progress, just activity.
		/// </summary>
        Marquee = 2,
    }

    /// <summary>
    /// Contains static methods used to show a progress dialog box.
    /// </summary>
    public static class ProgressDialog
    {
        /// <summary>
        /// Show the progress dialog to to the user.
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog.</param>
        public static void Show(BackgroundTask task, IDesktopWindow desktopWindow)
        {
            Show(task, desktopWindow, false, ProgressBarStyle.Blocks);
        }

        /// <summary>
        /// Show the progress dialog to to the user.
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog.</param>
        /// <param name="autoClose">Close the progress dialog after task completion.</param>
        public static void Show(BackgroundTask task, IDesktopWindow desktopWindow, bool autoClose)
        {
            Show(task, desktopWindow, autoClose, ProgressBarStyle.Blocks);
        }

        /// <summary>
        /// Show the progress dialog to to the user.
        /// </summary>
        /// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
        /// <param name="desktopWindow">Desktop window that parents the progress dialog.</param>
        /// <param name="autoClose">Close the progress dialog after task completion.</param>
        /// <param name="progressBarStyle">The style of the progress bar.</param>
        public static void Show(BackgroundTask task, IDesktopWindow desktopWindow, bool autoClose, ProgressBarStyle progressBarStyle)
        {
            ProgressDialogComponent progressComponent = new ProgressDialogComponent(task, autoClose, progressBarStyle);
            ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(
                desktopWindow,
                progressComponent,
                Application.Name);

            if (result == ApplicationComponentExitCode.Error)
                throw progressComponent.TaskException;
        }
    }
}
