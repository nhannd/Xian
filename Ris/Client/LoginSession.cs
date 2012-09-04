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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
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
		/// <param name="facilityCode"></param>
		public static void Create(string facilityCode)
		{
			// set the current session before attempting to access other services, as these will require authentication
			_current = new LoginSession(facilityCode);
		}


		private readonly string _facilityCode;
		private FacilitySummary _workingFacility;
		private bool _facilityLoaded;

		private StaffSummary _staff;
		private bool _staffLoaded;

		private LoginSession(string workingFacility)
		{
			_facilityCode = workingFacility;
		}


		/// <summary>
		/// Gets the user name of the logged on user.
		/// </summary>
		public string UserName
		{
			get { return GetThreadCredentials().UserName; }
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
			get
			{
				LoadFacilityOnce();
				return _workingFacility;
			}
		}

		/// <summary>
		/// Gets the session token.  This property is internal in order to limit exposure of the session
		/// token.
		/// </summary>
		public string SessionToken
		{
			get { return GetThreadCredentials().SessionTokenId; }
		}

		private void LoadStaffOnce()
		{
			if (_staffLoaded)
				return;

			Platform.GetService<IStaffAdminService>(
				service => _staff = service.ListStaff(new ListStaffRequest {UserName = UserName}).Staffs.FirstOrDefault());

			_staffLoaded = true;
		}

		private void LoadFacilityOnce()
		{
			if (_facilityLoaded)
				return;

			Platform.GetService<IFacilityAdminService>(
				service => _workingFacility = service.ListAllFacilities(new ListAllFacilitiesRequest())
					.Facilities.FirstOrDefault(f => f.Code == _facilityCode));

			_facilityLoaded = true;
		}

		private static IUserCredentialsProvider GetThreadCredentials()
		{
			var provider = Thread.CurrentPrincipal as IUserCredentialsProvider;
			if (provider == null)
				throw new InvalidOperationException("Thread.CurrentPrincipal value does not implement IUserCredentialsProvider.");

			return provider;
		}

	}
}
