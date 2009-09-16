using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    public partial class WorkQueueAlertPanel : System.Web.UI.UserControl
    {
        public string Text
        {
            get { return Message.Text; }
            set { Message.Text = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Visible = !String.IsNullOrEmpty(Message.Text);
            base.OnPreRender(e);
        }
    }
}