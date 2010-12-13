#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    //TODO: Do we still need this? or is it covered by ApplicationStartupParameters ?
    public class ServerConfiguration
    {
        public bool LANMode { get; set; }
        public TimeSpan InactivityTimeout { get; set; }
        public int Port { get; set; }

        public static ServerConfiguration Current { get; set; }
    }
}