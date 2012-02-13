#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public static class Constants
    {
        static public class SilverlightInitParameters
        {
			//TODO (CR May 2010) future: consolidate case - this looks a bit funny.
            public static string Username = "username";
			public static string IsSessionShared = "sessionshared";
			public static string Session = "session";
            public static string InactivityTimeout = "InactivityTimeout";
            public static string TimeoutUrl = "TimeoutUrl";
            public static string LogPerformance = "LogPerformance";
            public static string LocalIPAddress = "LocalIPAddress";
            public static string Mode = "Mode"; // see ApplicationServiceMode for the acceptable values
            public static string Language = "Language";			
        }
    }
}
