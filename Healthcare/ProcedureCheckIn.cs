#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// ProcedureCheckIn entity
	/// </summary>
	public partial class ProcedureCheckIn
	{
		/// <summary>
		/// Returns true if this procedure is pre check-in (patient has not yet checked-in).
		/// </summary>
		protected internal virtual bool IsPreCheckIn
		{
			get { return _checkInTime == null; }
		}

		/// <summary>
		/// Returns true if the patient is currently checked-in for this procedure.
		/// </summary>
		protected internal virtual bool IsCheckedIn
		{
			get { return !IsPreCheckIn && !IsCheckedOut; }
		}

		/// <summary>
		/// Returns true if the patient has checked-out for this procedure.
		/// </summary>
		protected internal virtual bool IsCheckedOut
		{
			get { return _checkOutTime != null; }
		}

		/// <summary>
		/// Check in the procedure, optionally specifying a check-in time.  If not specified,
		/// the current time is assumed.
		/// </summary>
		protected internal virtual void CheckIn(DateTime? checkInTime)
		{
			if (!IsPreCheckIn)
				throw new WorkflowException("Procedure already checked-in.");

			_checkInTime = checkInTime ?? Platform.Time;
		}

		/// <summary>
		/// Check out the procedure, optionally specifying a check-out time.  If not specified,
		/// the current time is assumed.
		/// </summary>
		protected internal virtual void CheckOut(DateTime? checkOutTime)
		{
			if (!IsCheckedIn)
				throw new WorkflowException("Procedure already checked-out.");

			_checkOutTime = checkOutTime ?? Platform.Time;
		}


		/// <summary>
		/// Reverts Check-In status if not already checked out
		/// </summary>
		protected internal virtual void RevertCheckIn()
		{
			if (!IsCheckedIn)
				throw new WorkflowException("Cannot revert check-in status of a procedure that is not currently checked-in.");

			_checkInTime = null;
		}

		/// <summary>
		/// Shifts the object in time by the specified number of days, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			_checkInTime = _checkInTime.HasValue ? _checkInTime.Value.AddMinutes(minutes) : _checkInTime;
			_checkOutTime = _checkOutTime.HasValue ? _checkOutTime.Value.AddMinutes(minutes) : _checkOutTime;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}