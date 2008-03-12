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

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    public partial class ConfirmationDialog : System.Web.UI.UserControl
    {
        /// <summary>
        /// Types of message to display.
        /// </summary>
        public enum MessageTypeEnum
        {
            NONE,
            WARNING
        } ;

        // The associated data. This can be retrieved later on.
        private object _data;
        private string _message;

        // type of message being displayed
        private MessageTypeEnum _type = MessageTypeEnum.NONE;

        /// <summary>
        /// Sets/Gets the associated data with the action.
        /// </summary>
        public object Data
        {
            set
            {
                _data = value;
                ViewState["ConfirmDialog_Data"] = value;
            }
            get { return _data; }
        }

        /// <summary>
        /// Sets/Gets the type of message being displayed.
        /// </summary>
        public MessageTypeEnum MessageType
        {
            set { _type = value; }
            get { return _type; }
        }


        /// <summary>
        /// Sets/Gets the messsage being displayed
        /// </summary>
        public string Message
        {
            set { _message = value; MessageLabel.Text = _message; }
            get { return _message;
                
            }
        }


        /// <summary>
        /// Defines the event handler for <seealso cref="Confirmed"/> event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void ConfirmedEventHandler(object data);

        /// <summary>
        /// Occurs when users click on "Yes" or "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                // Reload the data
                _data = ViewState["ConfirmDialog_Data"];
            }


        }

        
        protected void YesButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(_data);

            Close();
        }

        protected void NoButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Dismisses the confirmation box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();

        }

        public void Show()
        {
            switch (_type)
            {
                case MessageTypeEnum.NONE:
                    IconImage.Visible = false;
                    break;

                case MessageTypeEnum.WARNING:
                    IconImage.ImageUrl = "~/images/icons/icon_warning.png";
                    break;
            }

            ModalDialog1.Show();
        }

    }
}