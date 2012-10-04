using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace JuniorProjectWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnStudent_Click(object sender, EventArgs e)
        {
            Server.Transfer("Student.aspx");
        }

        protected void BtnTeacher_Click(object sender, EventArgs e)
        {
            Server.Transfer("Teacher.aspx");
        }

        protected void BtnAdmin_Click(object sender, EventArgs e)
        {
            Server.Transfer("Admin.aspx");
        }
    }
}
