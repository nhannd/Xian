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
    /// <summary>
    /// General dialog box to prompt user for confirmation.
    /// </summary>
    public partial class ConfirmDialog : System.Web.UI.UserControl
    {
        /// <summary>
        /// Types of message to display.
        /// </summary>
        public enum MessageTypeEnum
        {
            NONE,
            WARNING
        } ;


        #region Private Members
        
        // The associated data. This can be retrieved later on.
        private object _data;

        // type of message being displayed
        private MessageTypeEnum _type = MessageTypeEnum.NONE;
        
        #endregion Private Members


        #region Public properties

        /// <summary>
        /// Sets/Gets the associated data with the action.
        /// </summary>
        public object Data
        {
            set { 
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
            set { this.MessageLabel.Text = value;  }
            get { return this.MessageLabel.Text;  }
        }

        #endregion Public properties


        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="Confirmed"/> event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void ConfirmedEventHandler(object data);

        /// <summary>
        /// Occurs when users click on "Yes" or "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;

        #endregion Events

        #region Protected methods

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

        #endregion Protected methods

        

        #region public methods

        /// <summary>
        /// Dismisses the confirmation box.
        /// </summary>
        public void Close()
        {
            ModalPopupExtender1.Hide();
        }

        /// <summary>
        /// Display the confirmation box
        /// </summary>
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

            UpdatePanel.Update(); // force the client to refresh

            ModalPopupExtender1.Show();
        }

        #endregion public methods

    }
}