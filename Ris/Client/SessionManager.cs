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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;
using System.ServiceModel;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class LoginDialogExtensionPoint : ExtensionPoint<ILoginDialog>
    {
    }

    [ExtensionPoint]
    public class ChangePasswordDialogExtensionPoint : ExtensionPoint<IChangePasswordDialog>
    {
    }


    [ExtensionOf(typeof(SessionManagerExtensionPoint))]
    class SessionManager : ISessionManager
    {
        #region ISessionManager Members

        bool ISessionManager.InitiateSession()
        {
            try
            {
                return Login();
            }
            catch(Exception e)
            {
                // can't use ExceptionHandler here because no desktopWindow exists yet
                Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
                return false;
            }
        }

        void ISessionManager.TerminateSession()
        {
            try
            {
                LoginSession currentSession = LoginSession.Current;
                if (currentSession != null)
                {
                    currentSession.Terminate();
                }
            }
            catch (Exception e)
            {
                // since we're logging out, just log the exception and move on
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

        #region Internal methods

        internal static bool RenewLogin()
        {
            LoginSession current = LoginSession.Current;
            if (current == null)
                return false;

            return Login(LoginDialogMode.RenewLogin, current.UserName, current.WorkingFacility.Code);
        }

        internal static bool ChangePassword()
        {
            LoginSession current = LoginSession.Current;
            if(current == null)
                return false;

            string newPassword;
            return ChangePassword(current.UserName, null, out newPassword);
        }

        #endregion

        private static bool Login()
        {
            return Login(LoginDialogMode.InitialLogin, null, null);
        }

        private static bool Login(LoginDialogMode mode, string userName, string facility)
        {
            bool needLoginDialog = true;
            string password = null;

            Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
            List<FacilitySummary> facilities = GetFacilityChoices();

            Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
            while (true)
            {
                if (needLoginDialog)
                {
                    if (!ShowLoginDialog(mode, facilities, ref userName, ref facility, out password))
                    {
                        // user cancelled
                        return false;
                    }
                }

                try
                {
                    FacilitySummary selectedFacility = CollectionUtils.SelectFirst(facilities,
                        delegate(FacilitySummary fs) { return fs.Code == facility; });

                    // try to create the session
                    LoginSession.Create(userName, password, selectedFacility);

                    // successfully logged in
                    return true;
                }
                catch (PasswordExpiredException)
                {
                    string newPassword;
                    if(!ChangePassword(userName, password, out newPassword))
                    {
                        // user cancelled password change, so just abort everything
                        return false;
                    }

                    // loop again, but this time using the new password, and don't show the login dialog
                    // since we already have the credentials
                    password = newPassword;
                    needLoginDialog = false;
                }
                catch (Exception e)
                {
                    ReportException(e);
                }
            }
        }

        private static bool ShowLoginDialog(LoginDialogMode mode, List<FacilitySummary> facilities, ref string userName, ref string facility, out string password)
        {
            using (ILoginDialog loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
            {
                string[] facilityCodes = CollectionUtils.Map<FacilitySummary, string>(
                    facilities,
                    delegate(FacilitySummary fs) { return fs.Code; }).ToArray();

                string initialFacilityCode = LoginDialogSettings.Default.SelectedFacility;

                // if no saved facility, just choose the first one
                if (string.IsNullOrEmpty(initialFacilityCode) && facilityCodes.Length > 0)
                    initialFacilityCode = facilityCodes[0];

                loginDialog.Mode = mode;
                loginDialog.FacilityChoices = facilityCodes;
                loginDialog.Facility = initialFacilityCode;
                loginDialog.UserName = userName;

                Platform.Log(LogLevel.Debug, "Showing login dialog.");
                if (loginDialog.Show())
                {
                    // save selected facility
                    LoginDialogSettings.Default.SelectedFacility = loginDialog.Facility;
                    LoginDialogSettings.Default.Save();

                    userName = loginDialog.UserName;
                    password = loginDialog.Password;
                    facility = loginDialog.Facility;

                    return true;
                }
            }
            userName = null;
            password = null;
            facility = null;

            return false;
        }

        private static bool ChangePassword(string userName, string oldPassword, out string newPassword)
        {
            using (IChangePasswordDialog changePasswordDialog = (IChangePasswordDialog)(new ChangePasswordDialogExtensionPoint()).CreateExtension())
            {
                changePasswordDialog.UserName = userName;
                changePasswordDialog.Password = oldPassword;
                while (true)
                {
                    if (changePasswordDialog.Show())
                    {
                        try
                        {
                            LoginSession.ChangePassword(userName, changePasswordDialog.Password,
                                                changePasswordDialog.NewPassword);

                            newPassword = changePasswordDialog.NewPassword;
                            return true;
                        }
                        catch (Exception e)
                        {
                            ReportException(e);
                        }
                    }
                    else
                    {
                        // user cancelled
                        newPassword = null;
                        return false;
                    }
                }
            }
        }

        private static void ReportException(Exception e)
        {
            if(e is RequestValidationException)
            {
                ClearCanvas.Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
            }
            else if (e is CommunicationException)
            {
                Platform.Log(LogLevel.Error, e);
                ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
            }
            else if (e is TimeoutException)
            {
                Platform.Log(LogLevel.Error, e);
                ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
            }
            else
            {
                Platform.Log(LogLevel.Error, e);
                ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageUnknownErrorCommunicatingWithServer, MessageBoxActions.Ok);
            }
        }

        private static List<FacilitySummary> GetFacilityChoices()
        {
            List<FacilitySummary> choices = null;
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                {
                    choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
                });
            return choices;
        }
    }
}
