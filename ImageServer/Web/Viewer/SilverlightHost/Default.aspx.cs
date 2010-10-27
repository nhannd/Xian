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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Viewer.SilverlightHost
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string UserCredentialsString
        {
            get
            {
                return String.Format("username={0},session={1}", "admin", "123");
            }
        }

        public string ApplicationSettings
        {
            get
            {
                return String.Format("{0}={1}", "TimeoutUrl", "Timeout.aspx");
            }
        }

    }
}
