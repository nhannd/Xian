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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    public partial class ThrottleSettingsTab : System.Web.UI.UserControl
    {
        private const short UNLIMITED = -1;
        private short _maxConnections = UNLIMITED;

        public short MaxConnections
        {
            get{
                if (String.IsNullOrEmpty(MaxConnectionTextBox.Text))
                    return UNLIMITED;
                else
                    return short.Parse(MaxConnectionTextBox.Text);
            }
            set
            {
                MaxConnectionTextBox.Text = value==UNLIMITED? "":value.ToString();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UnlimitedCheckBox.Checked = MaxConnections == UNLIMITED;
            LimitedCheckBox.Checked = !UnlimitedCheckBox.Checked;
        }
    }
}