#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow {


    /// <summary>
    /// WorkQueueItem entity
    /// </summary>
	public partial class WorkQueueItem
	{
		public WorkQueueItem(string type) : this (
			Platform.Time,
			Platform.Time,
			null, 
			null,
			type,
			WorkQueueStatus.PN,
			null,
			0,
			null,
			new Dictionary<string, string>())
		{
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Marks the item as failed with the specified description, incrementing the failure count.
		/// </summary>
		/// <param name="description"></param>
		public virtual void Fail(string description)
		{
			_failureDescription = description.Length >= 1024 ? description.Substring(0, 1024) : description;
			_failureCount++;
			_status = WorkQueueStatus.F;
			_processedTime = Platform.Time;
		}

		/// <summary>
		/// Marks the item as complete.
		/// </summary>
		public virtual void Complete()
		{
			_status = WorkQueueStatus.CM;
			_processedTime = Platform.Time;
		}

		/// <summary>
		/// Reschedules the item for the specified time.
		/// </summary>
		/// <param name="time"></param>
		public virtual void Reschedule(DateTime time)
		{
			_status = WorkQueueStatus.PN;
			_scheduledTime = time;
		}

		/// <summary>
		/// Reschedules the item for the current time.
		/// </summary>
		public virtual void Reschedule()
		{
			Reschedule(Platform.Time);
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
			_scheduledTime = _scheduledTime.AddMinutes(minutes);

			_expirationTime = _expirationTime.HasValue ? _expirationTime.Value.AddMinutes(minutes) : _expirationTime;
			_processedTime = _processedTime.HasValue ? _processedTime.Value.AddMinutes(minutes) : _processedTime;
		}
	}

	public static class MailFaxWorkQueueItem
	{
		public static WorkQueueItem Create(
			string accessionNumber,
			EntityRef reportRef,
			EntityRef practitionerRef,
			EntityRef contactPointRef)
		{
			var workQueueItem = new WorkQueueItem("Mail/Fax Report");
			workQueueItem.ExtendedProperties.Add("AccessionNumber", accessionNumber);
			workQueueItem.ExtendedProperties.Add("ReportOID", reportRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerOID", practitionerRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerContactPointOID", contactPointRef.ToString(false, false));

			return workQueueItem;
		}
	}
}