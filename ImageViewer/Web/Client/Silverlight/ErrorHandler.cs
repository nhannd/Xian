#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public static class ErrorHandler
    {
        public static event EventHandler OnCriticalError;

        public static void HandleCriticalError(string message, params object[] args)
        {
            UIThread.Execute(() =>
            {
                if (OnCriticalError!=null)
                    OnCriticalError(null, EventArgs.Empty);

                var window= PopupHelper.PopupMessage("Error", string.Format(message, args));
                window.Closed += (s, e) => { CloseWindow(); };
            });
        }

        public static void HandleException(Exception ex)
        {
            UIThread.Execute(() =>
            {
                if (OnCriticalError != null)
                    OnCriticalError(null, EventArgs.Empty);

                var window = PopupHelper.PopupMessage("Error", ex.Message, "Close");
                window.Closed += (s, e) => { CloseWindow(); };
            });
        }

        private static void CloseWindow()
        {
            BrowserWindow.Close();
        }
    }
}
