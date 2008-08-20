using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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

		public virtual void Resubmit()
		{
			this.Status = WorkQueueStatus.PN;
			this.ScheduledTime = Platform.Time;
		}
	}

	public static class MailFaxWorkQueueItem
	{
		public static WorkQueueItem Schedule(
			string accessionNumber,
			Enterprise.Common.EntityRef reportRef,
			Enterprise.Common.EntityRef practitionerRef,
			Enterprise.Common.EntityRef contactPointRef,
			IPersistenceContext context)
		{
			WorkQueueItem workQueueItem = new WorkQueueItem("Mail/Fax Report");
			workQueueItem.ExtendedProperties.Add("AccessionNumber", accessionNumber);
			workQueueItem.ExtendedProperties.Add("ReportOID", reportRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerOID", practitionerRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerContactPointOID", contactPointRef.ToString(false, false));
			context.Lock(workQueueItem, DirtyState.New);

			return workQueueItem;
		}
	}
}