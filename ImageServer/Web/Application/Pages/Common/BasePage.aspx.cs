#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.Common;
using System.Threading;
using System.Globalization;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Common
{
    /// <summary>
    /// Base class for all the pages.
    /// </summary>
    /// <remarks>
    /// Derive new page from this class to ensure consistent look across all pages.
    /// </remarks>
    public partial class BasePage : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //Set the Page Theme, then set the Page object on the ImageServerConstants
            Page.Theme = ImageServerConstants.Default;
            
            // TODO: review this. It should be per session.
            ImageServerConstants.Theme = Page.Theme;

            HttpContext.Current.Items["Theme"] = Page.Theme;

            // This is necessary because Safari and Chrome browsers don't display the Menu control correctly.
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
              Page.ClientTarget = "uplevel";
        }

        protected void SetPageTitle(string title)
        {
            SetPageTitle(title, true);
        }

        private static string GetProductInformation()
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(ProductInformation.Release))
                tags.Add(App_GlobalResources.Titles.LabelNotForDiagnosticUse);
            if (!ServerPlatform.IsManifestVerified)
                // should be hardcoded because manifest verification is all that prevents localizing this tag away
                tags.Add("Modified Installation");

            var name = ProductInformation.GetName(false, true);
            if (tags.Count == 0)
                return name;

            var tagString = string.Join(" | ", tags.ToArray());
            return string.IsNullOrEmpty(name) ? tagString : string.Format("{0} - {1}", name, tagString);
        }

        protected void SetPageTitle(string title, bool includeProductInfo)
        {
            if (includeProductInfo)
            {
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                                 ? String.Format(title, GetProductInformation())
                                 : String.Format(title, GetProductInformation()) + " [" +
                                   ConfigurationManager.AppSettings["ServerName"] + "]";
            }
            else
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                    ? title
                    : title + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
		}
    }


}