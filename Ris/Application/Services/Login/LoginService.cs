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
using ClearCanvas.Enterprise.Core.ServiceModel;
using System;
using System.ServiceModel;

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
        	string hostName = StringUtilities.NullIfEmpty(request.HostName) ?? StringUtilities.NullIfEmpty(request.ClientIP);

            try
            {
                // initiate session and obtain authority tokens
                string[] authorityTokens = null;
                SessionToken token = InitiateSession(user, password, hostName, out authorityTokens);

                // store the working facility in the user's profile
                UpdateWorkingFacility(request.WorkingFacility);

                // load staff for user
                Staff staff = FindStaffForUser(user);

                return new LoginResponse(
                    token.Id,
                    authorityTokens,
                    staff == null ? null : new StaffAssembler().CreateStaffSummary(staff, this.PersistenceContext));

            }
            // for some reason, we need to catch and rethrow these to get the client
            // to see a strongly typed fault - otherwise it just gets a general FaultException
            catch (FaultException<UserAccessDeniedException> e)
            {
                throw e.Detail;
            }
            catch (FaultException<PasswordExpiredException> e)
            {
                throw e.Detail;
            }
        }


        [UpdateOperation]
        [Audit(typeof(LoginServiceRecorder))]
        public LogoutResponse Logout(LogoutRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");
            Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");

			try
			{
				Platform.GetService<IAuthenticationService>(
					delegate(IAuthenticationService service)
					{
						service.TerminateSession(new TerminateSessionRequest(request.UserName, new SessionToken(request.SessionToken)));
					});

				return new LogoutResponse();
			}
			catch (FaultException<InvalidUserSessionException> e)
			{
				throw e.Detail;
			}
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

                return new ChangePasswordResponse();
            }
            // for some reason, we need to catch and rethrow these to get the client
            // to see a strongly typed fault - otherwise it just gets a general FaultException
            catch (FaultException<RequestValidationException> e)
            {
                throw e.Detail;
            }
            catch (FaultException<UserAccessDeniedException> e)
            {
                throw e.Detail;
            }
        }

        #endregion

        /// <summary>
        /// Initiates a session for the specified user, and establishes a principal on this thread.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostname"></param>
        /// <param name="authorityTokens"></param>
        /// <returns></returns>
        private SessionToken InitiateSession(string userName, string password, string hostname, out string[] authorityTokens)
        {
            InitiateSessionResponse initSessionResponse = null;
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // TODO: app name shouldn't be hardcoded
                    initSessionResponse = service.InitiateSession(
                        new InitiateSessionRequest(userName, "RIS", hostname, password, true));

                    // setup a principal on this thread for the duration of this request
                    // (this is necessary in order to load the WorkingFacilitySettings, etc)
                    // TODO is this the best way to do this?  Seems a little hokey...
                    Thread.CurrentPrincipal = DefaultPrincipal.CreatePrincipal(new GenericIdentity(userName), initSessionResponse.SessionToken);
                });
            authorityTokens = initSessionResponse.AuthorityTokens;
            return initSessionResponse.SessionToken;
        }

        /// <summary>
        /// Updates the working facility stored in the user profile of the current user.
        /// </summary>
        /// <param name="facilityRef"></param>
        private void UpdateWorkingFacility(EntityRef facilityRef)
        {
            // store the working facility in the user's profile
            WorkingFacilitySettings settings = new WorkingFacilitySettings();
            if (facilityRef != null)
            {
                Facility facility = PersistenceContext.Load<Facility>(facilityRef);
                settings.WorkingFacilityCode = facility.Code;
            }
            else
            {
                // working facility not known
                settings.WorkingFacilityCode = "";
            }
            settings.Save();
        }

        /// <summary>
        /// Gets the staff associated with specified user, or null if no staff associated.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private Staff FindStaffForUser(string userName)
        {
            StaffSearchCriteria where = new StaffSearchCriteria();
            where.UserName.EqualTo(userName);
            return CollectionUtils.FirstElement(PersistenceContext.GetBroker<IStaffBroker>().Find(where));
        }

    }
}
