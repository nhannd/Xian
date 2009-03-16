using System;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    public partial class ThrottleSettingsTab : System.Web.UI.UserControl
    {
        private const short UNLIMITED = -1;

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