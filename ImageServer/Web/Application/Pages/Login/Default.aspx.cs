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
using System.ServiceModel;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class _Default : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            try
            {
                SessionInfo session = SessionManager.InitializeSession(UserName.Text, Password.Text);
				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource, 
					EventIdentificationTypeEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(session.Credentials.UserName, null, session.Credentials.DisplayName));
				ServerPlatform.LogAuditMessage("UserAuthentication", audit);

                Response.Redirect(FormsAuthentication.GetRedirectUrl(UserName.Text, false));
            }
            catch (PasswordExpiredException)
            {
                PasswordExpiredDialog.Show(UserName.Text, Password.Text);
            }
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Invalid login");

                // TODO: The server is throwing exception when username or password is invalid or something else. 
                // We can't determine the cause unless IncludeExceptionDetailInFaults is enabled on the server.
                // For this reason, we can only assume for now it only throw exceptions.
                // If the error is due to communication with the server, users will be redirected to the error page instead.
                ShowError(ErrorMessages.LoginInvalidUsernameOrPassword);
                UserName.Focus();

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated,UserAuthenticationEventType.Login );
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text,null,null));
				ServerPlatform.LogAuditMessage("UserAuthentication",audit);
			}
        }

        public void ChangePassword(object sender, EventArgs e)
        {
            ChangePasswordDialog.Show(true);
        }



        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
        }
    }
}
