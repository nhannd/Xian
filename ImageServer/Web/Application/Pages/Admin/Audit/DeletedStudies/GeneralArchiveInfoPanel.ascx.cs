using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class GeneralArchiveInfoPanel : BaseDeletedStudyArchiveUIPanel
    {
        public override void DataBind()
        {
            ArchiveXml.Text = XmlUtils.GetXmlDocumentAsString(ArchiveInfo.ArchiveXml, true);
            base.DataBind();
        }
    }
}