#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class UsersGuideLinkHelper
    {
        public static string HomeUrl
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(string.Format("~/Pages/Help/{0}", ClearCanvas.ImageServer.Web.Common.PageSettings.AboutPage.Default.UsersGuideUrl));
            }
        }

        public static string GetUrlTo(string topicID)
        {
            return string.Format("{0}?{1}.htm", HomeUrl, topicID);
        }

        public static string GetUrlTo(string topicID, string anchorID)
        {
            return string.Format("{0}#{1}", GetUrlTo(topicID), anchorID);
        }

    }
}
