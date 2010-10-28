#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Security.Permissions;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.View
{
    
    [PrincipalPermission(SecurityAction.Demand, Role = ImageServerConstants.WebViewerAuthorityToken)]
    public partial class Default : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetPageTitle(App_GlobalResources.Titles.ViewImagesPageTitle);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        	// for time-out to work, don't cache this page 
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        public string UserCredentialsString
        {
            get
            {
                return String.Format("{0}={1},{2}={3},{4}=true", 
                    ImageServerConstants.WebViewerStartupParameters.Username, SessionManager.Current.Credentials.UserName,
                    ImageServerConstants.WebViewerStartupParameters.Session, SessionManager.Current.Credentials.SessionToken.Id,
					ImageServerConstants.WebViewerStartupParameters.IsSessionShared);
            }
        }

        public string ApplicationSettings
        {
            get
            {
                return String.Format("{0}={1}", ImageServerConstants.WebViewerStartupParameters.TimeoutUrl, Page.ResolveUrl(ImageServerConstants.PageURLs.DefaultTimeoutPage));
            }
        }

        public string OtherParameters
        {
            get
            {
                return String.Format("{0}={1}", ImageServerConstants.WebViewerStartupParameters.LocalIPAddress, Request.UserHostAddress);
            }
        }

        protected void SetPageTitle(string title)
        {
            if (title.Contains("{0}"))
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]) ? String.Format(title, ProductInformation.GetNameAndVersion(false, true)) : String.Format(title, ProductInformation.GetNameAndVersion(false, true)) + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
            else
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]) ? title : title + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
        }
    }
}