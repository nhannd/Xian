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

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class JQuery : System.Web.UI.UserControl
    {
        private bool _maskedInput;
        public bool MaskedInput
        {
            get { return _maskedInput;  }
            set { _maskedInput = value; }
        }
                
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "jQuery", ResolveUrl("~/Scripts/jquery/jquery-1.2.6.min.js")); 

            if(MaskedInput)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MaskedInput", ResolveUrl("~/Scripts/jquery/jquery.maskedinput.js")); 
            }
        }
    }
}