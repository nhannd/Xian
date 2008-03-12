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

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    /// <summary>
    /// A dialog box that displays a message on the screen and waits for users to accept or reject.
    /// </summary>
    /// <remark>
    /// The confirmation dialog box presents users with two options: Yes and No. Applications should implement 
    /// an event handler for the <see cref="Confirmed"/> event which is fired when users press "Yes". The dialog 
    /// box will close automatically when users press "Yes" or "No".
    /// 
    /// </remark>
    public partial class ConfirmationDialog : System.Web.UI.UserControl
    {
        /// <summary>
        /// Types of confirmation message.
        /// </summary>
        public enum MessageTypeEnum
        {
            NONE,
            WARNING
        };

        #region Private Members
        private object _data;
        private string _message;
        private MessageTypeEnum _type = MessageTypeEnum.NONE;

        #endregion Private Members

        #region Public Properties

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
            get { 
                return _message;
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

        #endregion Protected Methods


        #region Public Methods

        /// <summary>
        /// Dismisses the confirmation box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();

        }

        /// <summary>
        /// Displays the confirmation message box
        /// </summary>
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

        #endregion Public Methods

    }
}