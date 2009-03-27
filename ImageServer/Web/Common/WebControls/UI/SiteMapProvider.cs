using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    class SiteMapProvider : XmlSiteMapProvider
    {
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            IList roles = node.Roles;

            if (roles == null || roles.Count == 0 || roles[0].Equals("*")) return true;

            foreach(string role in roles)
            {
                if(Thread.CurrentPrincipal.IsInRole(role.Trim()))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
