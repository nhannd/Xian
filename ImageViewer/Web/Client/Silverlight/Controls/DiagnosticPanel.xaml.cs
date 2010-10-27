#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows;
using System.Windows.Controls;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls
{
    public partial class DiagnosticPanel : UserControl
    {
        public DiagnosticPanel()
        {
            Logger.SetWriteMethod(LogMessage);
            Logger.SetErrorMethod(OnError);
            
            InitializeComponent();
        }

        private void OnError(string msg)
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           Log.Text += msg;
                                           DialogControl.Show("Error", msg, "Dismiss");
                                       });
        }

        private void LogMessage(string msg)
        {
            Dispatcher.BeginInvoke(() => { Log.Text += msg; });
        }


        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Log.Text = "";
        }
    }
}
