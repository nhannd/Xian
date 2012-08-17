#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Holds information related to the current login session.
	/// </summary>
	public sealed class LoginSession
	{
		private static LoginSession _current;

		/// <summary>
		/// Gets the current <see cref="LoginSession"/>.
		/// </summary>
		public static LoginSession Current
		{
			get { return _current; }
		}

		public static void Create(string userName, string password)
		{
			Create(userName, password, null, false);
		}

		/// <summary>
		/// Creates a new <see cref="LoginSession"/>.
		/// </summary>
		/// <remarks>
		/// Contacts the server and requests login using the specified credentials.  An exception will be thrown
		/// if the credentials are not valid.
		/// </remarks>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="facility"></param>
		/// <param name="risServerDown"></param>
		public static void Create(string userName, string password, FacilitySummary facility, bool risServerDown)
		{
			try
			{
				Platform.Log(LogLevel.Debug, "Attempting login...");

				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new InitiateSessionRequest(userName, GetMachineID(), Dns.GetHostName(), password) {GetAuthorizations = true};
						var response = service.InitiateSession(request);

						if (response.SessionToken == null)
							throw new Exception("Invalid session token returned from authentication service.");

						// if the call succeeded, set a default principal object on this thread, containing
						// the set of authority tokens for this user
						Thread.CurrentPrincipal = DefaultPrincipal.CreatePrincipal(
							new GenericIdentity(userName),
							response.SessionToken,
							response.AuthorityTokens);

						// set the current session before attempting to access other services, as these will require authentication
						_current = new LoginSession(userName, response.SessionToken, facility);
					});

				Platform.Log(LogLevel.Debug, "Login attempt was successful.");
			}
			catch (FaultException<UserAccessDeniedException> e)
			{
				Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
				throw e.Detail;
			}
			catch (FaultException<PasswordExpiredException> e)
			{
				Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
				throw e.Detail;
			}
		}

		internal static void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			try
			{
				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new ChangePasswordRequest(userName, oldPassword, newPassword);
						service.ChangePassword(request);
					});
			}
			catch (FaultException<UserAccessDeniedException> e)
			{
				throw e.Detail;
			}
			catch (FaultException<RequestValidationException> e)
			{
				throw e.Detail;
			}
		}

		private readonly string _userName;
		private readonly SessionToken _sessionToken;
		private readonly FacilitySummary _workingFacility;
		private StaffSummary _staff;
		private bool _staffLoaded;

		private LoginSession(string userName, SessionToken sessionToken, FacilitySummary workingFacility)
		{
			_userName = userName;
			_sessionToken = sessionToken;
			_workingFacility = workingFacility;
		}

		/// <summary>
		/// Terminates the current login session, setting the <see cref="Current"/> property to null.
		/// </summary>
		public void Terminate()
		{
			try
			{
				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new TerminateSessionRequest(_userName, _sessionToken);
						service.TerminateSession(request);
					});
			}
			finally
			{
				_current = null;
			}
		}

		/// <summary>
		/// Gets the user name of the logged on user.
		/// </summary>
		public string UserName
		{
			get { return _userName; }
		}

		/// <summary>
		/// Gets the full person name of the logged on user.
		/// </summary>
		public PersonNameDetail FullName
		{
			get
			{
				LoadStaffOnce();
				return _staff == null ? null : _staff.Name;
			}
		}

		/// <summary>
		/// Gets the <see cref="StaffSummary"/> of the logged on user.
		/// </summary>
		public StaffSummary Staff
		{
			get
			{
				LoadStaffOnce();
				return _staff;
			}
		}

		/// <summary>
		/// Gets if the user is associated with a RIS staff person.
		/// </summary>
		public bool IsStaff
		{
			get
			{
				LoadStaffOnce();
				return _staff != null;
			}
		}

		/// <summary>
		/// Gets the current working facility.
		/// </summary>
		public FacilitySummary WorkingFacility
		{
			get { return _workingFacility; }
		}

		/// <summary>
		/// Gets the session token.  This property is internal in order to limit exposure of the session
		/// token.
		/// </summary>
		internal string SessionToken
		{
			get { return _sessionToken.Id; }
		}

		private void LoadStaffOnce()
		{
			if (_staffLoaded)
				return;

			Platform.GetService<IStaffAdminService>(
				service => _staff = service.ListStaff(new ListStaffRequest {UserName = _userName}).Staffs.FirstOrDefault());

			_staffLoaded = true;
		}

		private static string GetMachineID()
		{
			try
			{
				// Use the serial number of the mother board
				string id = null;
				var mc = new ManagementClass("Win32_Baseboard");
				var moc = mc.GetInstances();
				foreach (ManagementObject mo in moc)
				{
					id = mo.Properties["SerialNumber"].Value.ToString().Trim();
					if (!string.IsNullOrEmpty(id))
						break;
				}

				return id;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
				return null;
			}
		}
	}
}
