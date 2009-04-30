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
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ProcedureCheckIn entity
    /// </summary>
	public partial class ProcedureCheckIn : ClearCanvas.Enterprise.Core.Entity
	{
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        #region Public Operations

		/// <summary>
		/// Check in the procedure, optionally specifying a check-in time.  If not specified,
		/// the current time is assumed.
		/// </summary>
		public virtual void CheckIn(DateTime? checkInTime)
		{
			if (_checkInTime != null)
				throw new WorkflowException("Procedure already checked-in.");

			_checkInTime = checkInTime ?? Platform.Time;
		}

        /// <summary>
		/// Check out the procedure, optionally specifying a check-out time.  If not specified,
		/// the current time is assumed.
        /// </summary>
		public virtual void CheckOut(DateTime? checkOutTime)
        {
			if (_checkOutTime != null)
				throw new WorkflowException("Procedure already checked-in.");

			_checkOutTime = checkOutTime ?? Platform.Time;
		}

		/// <summary>
		/// Returns true if this procedure is pre check-in (patient has not yet checked-in).
		/// </summary>
        public virtual bool IsPreCheckIn
        {
            get { return _checkInTime == null; }
        }

		/// <summary>
		/// Returns true if the patient is currently checked-in for this procedure.
		/// </summary>
    	public virtual bool IsCheckedIn
    	{
			get { return !IsPreCheckIn && !IsCheckedOut; }
    	}

		/// <summary>
		/// Returns true if the patient has checked-out for this procedure.
		/// </summary>
		public virtual bool IsCheckedOut
		{
			get { return _checkOutTime != null; }
		}

        #endregion

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
	}
}