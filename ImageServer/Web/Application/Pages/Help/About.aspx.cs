#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Help
{
    public partial class About : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(App_GlobalResources.Titles.AboutPageTitle);
        }

        protected bool EnterpriseMode
        {
            get
            {
                // There's no simple way to determine what mode the Web GUI is running.
                // Here we assume it's stand-alone if the DefaultAuthenticationService plugin is enabled.
                // This is not perfect but at least it works.                
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath("~/Web.Config"));
                XmlNode node = doc.SelectSingleNode("//extensions/extension[@class='ClearCanvas.ImageServer.Services.Common.Authentication.DefaultAuthenticationService, ClearCanvas.ImageServer.Services.Common']");
                return node != null && bool.Parse(node.Attributes["enabled"].Value) == false;
            }
        }
    }
}
