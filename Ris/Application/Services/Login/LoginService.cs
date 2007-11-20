#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Security.Principal;
using System.Text;
using System.Threading;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Login;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.IdentityModel.Tokens;

namespace ClearCanvas.Ris.Application.Services.Login
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ILoginService))]
    public class LoginService : ApplicationServiceBase, ILoginService
    {
        #region ILoginService Members

        [UpdateOperation]
        public LoginResponse Login(LoginRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");
            //Platform.CheckMemberIsSet(request.Password, "Password");

            string user = request.UserName;

            // obtain the set of authority tokens for the user
            // note that we don't need to be authenticated to access IAuthenticationService
            // because it will accessed in-process
            string[] authorityTokens = null;
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // TODO validate the password
                    if (!service.ValidateUser(user))
                        throw new SecurityTokenException("Invalid user name or password.");

                    authorityTokens = service.ListAuthorityTokensForUser(user);

                    // setup a generic principal on this thread for the duration of this request
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(user), authorityTokens);
                });

            if (request.WorkingFacility != null)
            {
                Facility facility = PersistenceContext.Load<Facility>(request.WorkingFacility);

                WorkingFacilitySettings settings = new WorkingFacilitySettings();
                settings.WorkingFacility = facility.Code;
                settings.WorkingInformationAuthority = facility.InformationAuthority.Code;
                settings.Save();
            }


            // obtain full name information for the user
            PersonNameDetail fullName = null;
            try 
	        {	
                Staff staff = PersistenceContext.GetBroker<IStaffBroker>().FindStaffForUser(user);
                if(staff != null)
                {
                    PersonNameAssembler nameAssembler = new PersonNameAssembler();
                    fullName = nameAssembler.CreatePersonNameDetail(staff.Name);
                }

	        }
	        catch (EntityNotFoundException)
	        {
                // no staff associated to user - can't return full name details to client
	        }

            //TODO: provide session token
            return new LoginResponse("none", authorityTokens, fullName);
        }

        [ReadOperation]
        public GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request)
        {
            // facility choices - for now, just return all facilities
            // conceivably this list could be filtered for various reasons
            // (ie inactive facilities, etc) 
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            List<FacilitySummary> facilities = CollectionUtils.Map<Facility, FacilitySummary>(
                PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                delegate(Facility f) { return facilityAssembler.CreateFacilitySummary(f); });

            return new GetWorkingFacilityChoicesResponse(facilities);
        }

        #endregion
    }
}
