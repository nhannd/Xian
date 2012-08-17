#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
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

		/// <summary>
		/// Creates a new <see cref="LoginSession"/>.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="sessionToken"></param>
		/// <param name="facility"></param>
		public static void Create(string userName, SessionToken sessionToken, FacilitySummary facility)
		{
			// set the current session before attempting to access other services, as these will require authentication
			_current = new LoginSession(userName, sessionToken, facility);
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
		internal SessionToken SessionToken
		{
			get { return _sessionToken; }
		}

		private void LoadStaffOnce()
		{
			if (_staffLoaded)
				return;

			Platform.GetService<IStaffAdminService>(
				service => _staff = service.ListStaff(new ListStaffRequest {UserName = _userName}).Staffs.FirstOrDefault());

			_staffLoaded = true;
		}

	}
}
