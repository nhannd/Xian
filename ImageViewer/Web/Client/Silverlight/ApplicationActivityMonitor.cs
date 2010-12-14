#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class ApplicationActivityMonitor
    {
        static ApplicationActivityMonitor _instance = new ApplicationActivityMonitor();
        static public ApplicationActivityMonitor Instance
        {
            get { return _instance; }
        }


        long _lastActivityTick = Environment.TickCount;

        private ApplicationActivityMonitor() { }


        public long LastActivityTick
        {
            get { return _lastActivityTick; }
            set { _lastActivityTick = value; }
        }
    }
}
