#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
            InitializeComponent();
            
            ApplicationLog.LogAppended += new EventHandler<ApplicationLogAppendEventArgs>(ApplicationLog_LogAppended);
            Log.Text = ApplicationLog.GetLogContent();
        }

        void ApplicationLog_LogAppended(object sender, ApplicationLogAppendEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Log.Text += e.Message;
            });
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            ApplicationLog.Clear();
            Log.Text = "";
        }
        
    }
}
