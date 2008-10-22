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
    public partial class EmptySearchResultsMessage : System.Web.UI.UserControl
    {
        private string _message = String.Empty;

        public string Message
        { 
            get { return _message;}
            set{ _message = value;}
        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            ResultsMessage.Text = Message;
        }
    }
}