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

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class LoginDialogExtensionPoint : ExtensionPoint<ILoginDialog>
    {
    }


    [ExtensionOf(typeof(SessionManagerExtensionPoint))]
    class SessionManager : ISessionManager
    {
        #region ISessionManager Members

        public bool InitiateSession()
        {
            try
            {
                return DoLogin();
            }
            catch(Exception e)
            {
                // can't use ExceptionHandler here because no desktopWindow exists yet
                Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
                return false;
            }
        }

        public void TerminateSession()
        {
        }

        #endregion


        private bool DoLogin()
        {
            using (ILoginDialog loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
            {
                List<FacilitySummary> facilities = GetFacilityChoices();
                string[] facilityCodes = CollectionUtils.Map<FacilitySummary, string>(
                    facilities,
                    delegate(FacilitySummary fs) { return fs.Code; }).ToArray();

                string selectedFacilityCode = LoginDialogSettings.Default.SelectedFacility;

                // if no saved facility, just choose the first one
                if (string.IsNullOrEmpty(selectedFacilityCode) && facilityCodes.Length > 0)
                    selectedFacilityCode = facilityCodes[0];


                loginDialog.SetFacilityChoices(facilityCodes, selectedFacilityCode);

                while (true)
                {
                    string userName;
                    string password;

                    if (loginDialog.Show(out userName, out password, out selectedFacilityCode))
                    {
                        FacilitySummary selectedFacility = CollectionUtils.SelectFirst(facilities,
                            delegate(FacilitySummary fs) { return fs.Code == selectedFacilityCode; });

                        try
                        {
                            LoginSession.Create(userName, password, selectedFacility);

                            // successfully logged in

                            // save selected facility
                            LoginDialogSettings.Default.SelectedFacility = selectedFacilityCode;
                            LoginDialogSettings.Default.Save();

                            return true;
                        }
                        catch(FaultException<RequestValidationException> e)
                        {
                            ClearCanvas.Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
                        }
                        catch (CommunicationException e)
                        {
                            Platform.Log(LogLevel.Error, e);
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
                        }
                        catch (TimeoutException e)
                        {
                            Platform.Log(LogLevel.Error, e);
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e);
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageUnknownErrorCommunicatingWithServer, MessageBoxActions.Ok);
                        }
                    }
                    else
                    {
                        // user cancelled
                        return false;
                    }
                }
            }
        }

        private List<FacilitySummary> GetFacilityChoices()
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
