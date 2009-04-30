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

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;
using ChangePasswordRequest=ClearCanvas.Ris.Application.Common.Login.ChangePasswordRequest;
using ChangePasswordResponse=ClearCanvas.Ris.Application.Common.Login.ChangePasswordResponse;

namespace ClearCanvas.Ris.Application.Services.Login
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ILoginService))]
    public class LoginService : ApplicationServiceBase, ILoginService
    {
        #region ILoginService Members

        [ReadOperation]
        public GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request)
        {
            // facility choices - for now, just return all facilities
            // conceivably this list could be filtered for various reasons
            // (ie inactive facilities, etc) 
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            List<FacilitySummary> facilities = CollectionUtils.Map<Facility, FacilitySummary>(
				PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
                delegate(Facility f) { return facilityAssembler.CreateFacilitySummary(f); });

            return new GetWorkingFacilityChoicesResponse(facilities);
        }

        [UpdateOperation]
        [Audit(typeof(LoginServiceRecorder))]
        public LoginResponse Login(LoginRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");

            string user = request.UserName;
            string password = StringUtilities.EmptyIfNull(request.Password);

            // obtain the set of authority tokens for the user
            // note that we don't need to be authenticated to access IAuthenticationService
            // because it will accessed in-process
            string[] authorityTokens = null;
            SessionToken token = null;
            try
            {
                Platform.GetService<IAuthenticationService>(
                    delegate(IAuthenticationService service)
                    {
                        // this call will throw SecurityTokenException if user/password not valid
                        // it will throw a PasswordExpiredException if the password has expired
                        // note that PasswordExpiredException is part of the fault contract of this method,
                        // so we don't catch it

                        token = service.InitiateSession(new InitiateSessionRequest(user, password)).SessionToken;

                        authorityTokens = service.GetAuthorizations(new GetAuthorizationsRequest(user, token)).AuthorityTokens;

                        // setup a generic principal on this thread for the duration of this request
                        // (this is necessary in order to load the WorkingFacilitySettings below)
                        Thread.CurrentPrincipal = new GenericPrincipal(
                            new GenericIdentity(user), authorityTokens);
                    });

            }
            catch (SecurityTokenException e)
            {
                // login failed
                throw new RequestValidationException(e.Message);
            }

            // store the working facility in the user's profile
            WorkingFacilitySettings settings = new WorkingFacilitySettings();
            if (request.WorkingFacility != null)
            {
                Facility facility = PersistenceContext.Load<Facility>(request.WorkingFacility);
                settings.WorkingFacilityCode = facility.Code;
            }
            else
            {
                // working facility not known
                settings.WorkingFacilityCode = "";
            }
            settings.Save();


        	StaffSummary staffSummary = null;
            try 
            {	
                StaffSearchCriteria where = new StaffSearchCriteria();
                where.UserName.EqualTo(user);
                Staff staff = PersistenceContext.GetBroker<IStaffBroker>().FindOne(where);
                staffSummary = staff == null ? null : new StaffAssembler().CreateStaffSummary(staff, this.PersistenceContext);
            }
            catch (EntityNotFoundException)
            {
                // no staff associated to user 
            }

            return new LoginResponse(token.Id, authorityTokens, staffSummary);
        }

        [UpdateOperation]
        [Audit(typeof(LoginServiceRecorder))]
        public LogoutResponse Logout(LogoutRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");
            Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // this call will throw SecurityTokenException if user/session token not valid
                    // we intentionally don't catch this, allowing it to cause a fault
                    service.TerminateSession(new TerminateSessionRequest(request.UserName, new SessionToken(request.SessionToken)));
                });

            return new LogoutResponse();
        }

        [UpdateOperation]
        [Audit(typeof(LoginServiceRecorder))]
        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");

            string user = request.UserName;
            string password = StringUtilities.EmptyIfNull(request.Password);
            string newPassword = StringUtilities.EmptyIfNull(request.NewPassword);

            try
            {
                Platform.GetService<IAuthenticationService>(
                    delegate(IAuthenticationService service)
                    {
                        // this call will throw SecurityTokenException if user/password not valid
                        service.ChangePassword(new Enterprise.Common.Authentication.ChangePasswordRequest(user, password, newPassword));
                    });

            }
            catch (SecurityTokenException e)
            {
                // login failed
                throw new RequestValidationException(e.Message);
            }

            return new ChangePasswordResponse();
        }

        #endregion
    }
}
