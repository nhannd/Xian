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
    public class StartupArguments
    {
        public StartupArguments(IDictionary<String, String> args)
        {
            InactivityTimeout = args.ContainsKey("InactivityTimeout") ? TimeSpan.Parse(args["InactivityTimeout"]) : TimeSpan.FromMinutes(60);

            if (args.ContainsKey("LANMode"))
            {
                bool isLAN;
                if (bool.TryParse(args["LANMode"], out isLAN))
                {
                    LANMode = isLAN;
                }
            }

            if (args.ContainsKey("Port"))
            {
                int port;
                if (int.TryParse(args["Port"], out port))
                {
                    Port = port;
                }

            }
        }
        
        public TimeSpan InactivityTimeout { get; set; }
        public bool LANMode { get; set; }
        public int Port { get; set; }
    }
}
