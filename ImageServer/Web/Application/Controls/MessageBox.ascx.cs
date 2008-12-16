#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// A dialog box that displays a message on the screen and waits for users to accept or reject.
    /// </summary>
    /// <remark>
    /// <para>
    /// The confirmation dialog box presents users with different buttons depending on the <see cref="MessageType"/>. For eg,
    /// if  <see cref="MessageType"/> is set to <see cref="MessageTypeEnum.YESNO"/>, "Yes" and "No" buttons will be displayed
    /// below the <see cref="Message"/>. If <see cref="MessageType"/> is set to <see cref="MessageTypeEnum.INFORMATION"/>, only
    /// "OK" button will be displayed.
    /// </para>
    /// <para>
    /// Unless explicitly set by the applications, the <see cref="Title"/> of the dialog box will be set based on the <see cref="MessageType"/>
    /// </para>
    /// <para>
    /// Applications should implement an event handler for the <see cref="Confirmed"/> event which is fired 
    /// when users press "Yes" (or equivalent buttons). The dialog box will close automatically when users press on the buttons.
    /// </para>
    /// </remark>
    public partial class MessageBox : System.Web.UI.UserControl
    {
        /// <summary>
        /// Types of confirmation message.
        /// </summary>
        public enum MessageTypeEnum
        {
            YESNO,  // displays "Yes" and "No" buttons
            OKCANCEL, // displays "OK" and "Cancel" buttons
            INFORMATION, // displays "OK" button.
            ERROR,      // displays "OK" button
            NONE            // do not display any button
        };

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
                ViewState["Data"] = value;
            }
            get
            {
                return ViewState["Data"];
            }
        }

        /// <summary>
        /// Sets/Gets the type of message being displayed.
        /// </summary>
        public MessageTypeEnum MessageType
        {
            set { 
                ViewState["MsgType"] = value;
            }
            get
            {
                if (ViewState["MsgType"] == null)
                    return MessageTypeEnum.NONE;
                else
                    return (MessageTypeEnum) ViewState["MsgType"];
            }
        }

        /// <summary>
        /// Sets/Gets the a background css 
        /// </summary>
        public string BackgroundCSS
        {
            set {  ModalDialog.BackgroundCSS = value; }
            get { return ModalDialog.BackgroundCSS; }
        }


        /// <summary>
        /// Sets/Gets the messsage being displayed
        /// </summary>
        public string Message
        {
            set { 
                _message = value;  }
            get { 
                return _message;
            }
        }

        /// <summary>
        /// Sets/Gets the title of the dialog box.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { 
                _title = value;
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
        /// Occurs when users click on "Yes" or "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;

        #endregion Events

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            YesButton.Visible = false;
            NoButton.Visible = false;
            OKButton.Visible = false;
            CancelButton.Visible = false;

            
            switch (MessageType)
            {
                case MessageTypeEnum.ERROR:
                    OKButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = App_GlobalResources.SR.ConfirmDialogError;
                    break;

                case MessageTypeEnum.INFORMATION:
                    OKButton.Visible = true;
                    break;

                case MessageTypeEnum.OKCANCEL:
                    OKButton.Visible = true;
                    CancelButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = App_GlobalResources.SR.ConfirmDialogDefault;
                    break;

                case MessageTypeEnum.YESNO:
                    YesButton.Visible = true;
                    NoButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = App_GlobalResources.SR.ConfirmDialogDefault;
                    break;

                default:
                    break;
            }

            MessageLabel.Text = _message;
            ModalDialog.Title = _title;

            base.OnPreRender(e);
        }

        protected void YesButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(Data);

            Close();
        }


        protected void NoButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(Data);

            Close();
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
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