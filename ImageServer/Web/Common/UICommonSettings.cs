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
using System.Text;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    public static class UICommonSettings
    {
        public static class  Admin
        {
            public static class Device
            {
                public static short MaxConnections
                {
                    get
                    {
                        return ImageServerCommonConfiguration.Device.MaxConnections;
                    }
                }
            }
        }
    }
}
