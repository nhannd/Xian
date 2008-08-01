using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// WorkQueue entity
	/// </summary>
	public partial class WorkQueue : ClearCanvas.Enterprise.Core.Entity
	{
		public WorkQueue(WorkQueueType type) : this (
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
}