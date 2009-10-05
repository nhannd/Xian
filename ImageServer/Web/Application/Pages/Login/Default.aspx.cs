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
using System.ServiceModel;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
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

            SetPageTitle(Titles.LoginPageTitle);

        }

        protected bool EnterpriseMode
        {
            get
            {
            	// There's no simple way to determine what mode the Web GUI is running.
            	// Here we assume it's stand-alone if the DefaultAuthenticationService plugin is enabled.
                // This is not perfect but at least it works.                
            	XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath("~/Web.Config"));
                XmlNode node = doc.SelectSingleNode("//extensions/extension[@class='ClearCanvas.ImageServer.Services.Common.Authentication.DefaultAuthenticationService, ClearCanvas.ImageServer.Services.Common']");
                return node != null && bool.Parse(node.Attributes["enabled"].Value) == false;
            }
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            try
            {
                SessionManager.InitializeSession(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationTypeEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, SessionManager.Current.Credentials.DisplayName));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (PasswordExpiredException)
            {
                Platform.Log(LogLevel.Info, "Password for {0} has expired. Requesting new password.",UserName.Text);
                PasswordExpiredDialog.Show(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationTypeEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (UserAccessDeniedException ex)
            {
                Platform.Log(LogLevel.Error, ex, ex.Message);
                ShowError(ex.Message);
                UserName.Focus();

                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
                    EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated, UserAuthenticationEventType.Login);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
                ServerPlatform.LogAuditMessage(audit);
            }
            catch (CommunicationException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unable to contact A/A server");
                ShowError(ErrorMessages.CannotContactEnterpriseServer);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Login error:");
                ShowError(ex.Message);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
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
