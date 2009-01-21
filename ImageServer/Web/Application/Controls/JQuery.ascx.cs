using System;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class JQuery : System.Web.UI.UserControl
    {
        private bool _multiselect = false;
    	private bool _maskedinput = false;

        public bool MultiSelect
        {
            get { return _multiselect;  }
            set { _multiselect = value; }
        }

		public bool MaskedInput
		{
			get { return _maskedinput; }
			set { _maskedinput = value; }
		}
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "jQuery", ResolveUrl("~/Scripts/jquery/jquery-1.2.6.min.js"));

            //Default Libraries
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "ClearCanvas", ResolveUrl("~/Scripts/ClearCanvas.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "DropShadow", ResolveUrl("~/Scripts/jquery/jquery.dropshadow.js")); 

            if(MultiSelect)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "Dimensions", ResolveUrl("~/Scripts/jquery/jquery.dimensions.js"));
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MultiSelect", ResolveUrl("~/Scripts/jquery/jquery.multiselect.js")); 
            }

			if (MaskedInput)
			{
				Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MaskedInput", ResolveUrl("~/Scripts/jquery/jquery.maskedinput-1.2.1.js"));
			}
		}
    }
}