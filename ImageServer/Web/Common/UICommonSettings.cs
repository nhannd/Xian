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
