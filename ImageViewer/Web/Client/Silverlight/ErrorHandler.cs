#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Web.Client.Silverlight.Utilities;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public static class ErrorHandler
    {
        public static event EventHandler OnCriticalError;

        public static void HandleCriticalError(string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            UIThread.Execute(() =>
            {
                if (OnCriticalError != null)
                    OnCriticalError(formattedMessage, EventArgs.Empty);               
            });
        }

        public static void HandleException(Exception ex)
        {
            UIThread.Execute(() =>
            {
                if (OnCriticalError != null)
                    OnCriticalError(ex.Message, EventArgs.Empty);
            });
        }
    }
}
