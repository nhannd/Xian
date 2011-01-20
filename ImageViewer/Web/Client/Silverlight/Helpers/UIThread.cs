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
using System.Threading;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    public static class UIThread
    {
        /// <summary>
        /// Helper class to ensure an action is executed on the UI thread. 
        /// If the current thread is no the UI thread, the action will be queued for execution.
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
                action.Invoke();
            else
                Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
