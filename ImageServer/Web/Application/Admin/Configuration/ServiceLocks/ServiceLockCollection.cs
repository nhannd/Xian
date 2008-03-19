using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Common;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks
{
    public class ServiceLockCollection:EntityCollection<ServiceLock>
    {
    }
}
