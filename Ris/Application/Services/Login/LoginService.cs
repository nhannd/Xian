#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Login;
using ChangePasswordRequest = ClearCanvas.Ris.Application.Common.Login.ChangePasswordRequest;
using ChangePasswordResponse = ClearCanvas.Ris.Application.Common.Login.ChangePasswordResponse;
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
			var facilityAssembler = new FacilityAssembler();
			var facilities = CollectionUtils.Map(
				PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
				(Facility input) => facilityAssembler.CreateFacilitySummary(input));

			return new GetWorkingFacilityChoicesResponse(facilities);
		}

		[UpdateOperation]
		[Audit(typeof(LoginServiceRecorder))]
		public LoginResponse Login(LoginRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var user = request.UserName;
			var password = StringUtilities.EmptyIfNull(request.Password);
			var hostName = StringUtilities.NullIfEmpty(request.HostName) ?? StringUtilities.NullIfEmpty(request.ClientIP);

			try
			{
				// initiate session and obtain authority tokens
				string[] authorityTokens;
				var token = InitiateSession(user, password, hostName, out authorityTokens);

				// load staff for user
				var staff = FindStaffForUser(user);

				return new LoginResponse(
					token,
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
					service => service.TerminateSession(new TerminateSessionRequest(request.UserName, request.SessionToken)));

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

			var user = request.UserName;
			var password = StringUtilities.EmptyIfNull(request.Password);
			var newPassword = StringUtilities.EmptyIfNull(request.NewPassword);

			try
			{
				Platform.GetService<IAuthenticationService>(
					service =>
					service.ChangePassword(new Enterprise.Common.Authentication.ChangePasswordRequest(user, password, newPassword)));

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
		private static SessionToken InitiateSession(string userName, string password, string hostname, out string[] authorityTokens)
		{
			InitiateSessionResponse initSessionResponse = null;
			Platform.GetService<IAuthenticationService>(
				service =>
				{
					// TODO: app name shouldn't be hardcoded
					initSessionResponse = service.InitiateSession(
						new InitiateSessionRequest(userName, "RIS", hostname, password, true));

					// setup a principal on this thread for the duration of this request
					// (this is necessary in order to load the WorkingFacilitySettings, etc)
					Thread.CurrentPrincipal = DefaultPrincipal.CreatePrincipal(new GenericIdentity(userName), initSessionResponse.SessionToken);
				});
			authorityTokens = initSessionResponse.AuthorityTokens;
			return initSessionResponse.SessionToken;
		}

		/// <summary>
		/// Gets the staff associated with specified user, or null if no staff associated.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		private Staff FindStaffForUser(string userName)
		{
			var where = new StaffSearchCriteria();
			where.UserName.EqualTo(userName);
			return CollectionUtils.FirstElement(PersistenceContext.GetBroker<IStaffBroker>().Find(where));
		}

	}
}
