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
using System.Configuration;
using System.Web;

namespace ClearCanvas.ImageServer.Web.Viewer.SilverlightHost
{
    public class WebServiceConfiguration
    {

        public static string InitParamString
        {
            get
            {
#if SILVERLIGHT4
                return String.Format("LANMode={0},Port={1},InactivityTimeout={2}",
                                     true, ConfigurationManager.AppSettings["Port"],
                                     ConfigurationManager.AppSettings["InactivityTimeout"]);
#else
                return String.Format("LANMode={0},Port={1},InactivityTimeout={2}",
                                     false, ConfigurationManager.AppSettings["Port"],
                                     ConfigurationManager.AppSettings["InactivityTimeout"]);
#endif

            }
        }
    }
}
