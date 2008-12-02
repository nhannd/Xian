using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// WorkQueueItem entity
    /// </summary>
	public partial class WorkQueueItem : ClearCanvas.Enterprise.Core.Entity
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
			_failureDescription = description;
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
	}

	public static class MailFaxWorkQueueItem
	{
		public static WorkQueueItem Create(
			string accessionNumber,
			EntityRef reportRef,
			EntityRef practitionerRef,
			EntityRef contactPointRef)
		{
			WorkQueueItem workQueueItem = new WorkQueueItem("Mail/Fax Report");
			workQueueItem.ExtendedProperties.Add("AccessionNumber", accessionNumber);
			workQueueItem.ExtendedProperties.Add("ReportOID", reportRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerOID", practitionerRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerContactPointOID", contactPointRef.ToString(false, false));

			return workQueueItem;
		}
	}
}