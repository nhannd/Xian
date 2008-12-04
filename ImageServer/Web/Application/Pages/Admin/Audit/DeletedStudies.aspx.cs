using System;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit
{
    public partial class DeletedStudies : BaseAdminPage
    {
        
        protected override void OnLoad(EventArgs e)
        {
           
            base.OnLoad(e);
            DataBind();
        }

    }
}
