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
    public partial class TimedDialog : System.Web.UI.UserControl
    {
        #region Private Members
        private string _message;
        private string _title;
       
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the associated data with the action.
        /// </summary>
        public object Data
        {
            set
            {
                ViewState[ClientID + "_Data"] = value;
            }
            get
            {
                return ViewState[ClientID + "_Data"];
            }
        }

        /// <summary>
        /// Sets/Gets the a background css 
        /// </summary>
        public string BackgroundCSS
        {
            set { ModalDialog.BackgroundCSS = value; }
            get { return ModalDialog.BackgroundCSS; }
        }


        /// <summary>
        /// Sets/Gets the messsage being displayed
        /// </summary>
        public string Message
        {
            set
            {
                _message = value;
                MessageLabel.Text = _message;
            }
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Sets/Gets the title of the dialog box.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                ModalDialog.Title = value;
            }
        }

        #endregion Public Properties

        #region Events


        /// <summary>
        /// Defines the event handler for <seealso cref="Confirmed"/> event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void ConfirmedEventHandler(object data);

        /// <summary>
        /// Occurs when users click on "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;


        #endregion Events

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            OKButton.Visible = true;
          
            base.OnPreRender(e);
        }

   

        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(Data);

            Close();
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Dismisses the confirmation box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();
        }

        /// <summary>
        /// Displays the confirmation message box
        /// </summary>
        public void Show()
        {
            ModalDialog.Show();
        }

        #endregion Public Methods
    }
}