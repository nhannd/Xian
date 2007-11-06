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

namespace ImageServerWebApplication.Common
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConfirmDialog : System.Web.UI.UserControl
    {
        public enum MessageTypeEnum
        {
            NONE,
            WARNING
        } ;

        private MessageTypeEnum _type = MessageTypeEnum.NONE;
        public MessageTypeEnum MessageType
        {
            set { _type = value; }
            get { return _type; }
        }

        public string Message
        {
            set { this.MessageLabel.Text = value;  }
            get { return this.MessageLabel.Text;  }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public delegate void ConfirmedDelegate();

        public ConfirmedDelegate OnConfirmed;

        public void Close()
        {
            ModalPopupExtender1.Hide();
        }

        public void Show()
        {
            switch(_type)
            {
                case MessageTypeEnum.NONE:
                        IconImage.Visible = false;
                        break;

                case MessageTypeEnum.WARNING:
                    IconImage.ImageUrl = "~/images/icon_warning2.gif";
                    break;
            }

            ModalPopupExtender1.Show();
        }

        protected void YesButton_Click(object sender, EventArgs e)
        {
            if (OnConfirmed != null)
                OnConfirmed();

            Close();
        }

    }
}