using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Common
{
    public static class UserProfile
    {
        /// <summary>
        /// Returns the default url for the current user
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultUrl()
        {
            //TODO: Use a mapping file similar to SiteMap to specify the default home page based on the authority tokens that user has.
            
            if (SessionManager.Current == null)
            {
                // user has not logged in
                FormsAuthentication.RedirectToLoginPage();
            }

            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Admin.System.EnterpriseConfiguration))
            {
                return ImageServerConstants.PageURLs.AdminUserPage;
            }
            else
            {
                return ImageServerConstants.PageURLs.SearchPage;
            }
        }
    }
}
