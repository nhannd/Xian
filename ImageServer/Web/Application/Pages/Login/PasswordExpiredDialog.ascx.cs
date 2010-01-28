#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class PasswordExpiredDialog : UserControl
    {
        public void Show(string username, string password)
        {
            Username.Text = username;
            OriginalPassword.Value = password;
            ErrorMessagePanel.Visible = false;
            Panel1.DefaultButton = "OKButton";
            NewPassword.Focus();
            ModalDialog1.Show();
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            Panel1.DefaultButton = "";
            ModalDialog1.Hide();
        }

        public void ChangePassword_Click(object sender, EventArgs e)
        {
            using(LoginService service = new LoginService())
            {
                try
                {
                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text =
                            "Provided passwords don't match.<br/>Please retype your passwords and try again.";
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(Username.Text, OriginalPassword.Value, NewPassword.Text);
                        SessionManager.InitializeSession(Username.Text, NewPassword.Text);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.Text = ex.Message;
                    ErrorMessagePanel.Visible = true;
                    NewPassword.Focus();
					// May want to elimiate this.
					Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
				}
            }
        }
    }
}