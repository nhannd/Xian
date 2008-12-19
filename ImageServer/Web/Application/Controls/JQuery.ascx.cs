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
        private bool _multiselect;
        private bool _effects;

        public bool MultiSelect
        {
            get { return _multiselect;  }
            set { _multiselect = value; }
        }

        public bool Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }
                
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "jQuery", ResolveUrl("~/Scripts/jquery/jquery-1.2.6.min.js")); 

            if(MultiSelect)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "Dimensions", ResolveUrl("~/Scripts/jquery/jquery.dimensions.js"));
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MultiSelect", ResolveUrl("~/Scripts/jquery/jquery.multiselect.js")); 
            }

            if(Effects)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "Effects", ResolveUrl("~/Scripts/Effects.js")); 
            }
        }
    }
}