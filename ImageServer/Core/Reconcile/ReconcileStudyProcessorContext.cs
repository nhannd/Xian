#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Encapsulated the context of the image reconciliation operation.
	/// </summary>
	public class ReconcileStudyProcessorContext
	{
		#region Public Properties

		/// <summary>
		/// The 'ReconcileStudy' <see cref="WorkQueue"/> item.
		/// </summary>
		public WorkQueue WorkQueueItem { get; set; }

		/// <summary>
		/// The server partition associated with <see cref="WorkQueueItem"/>
		/// </summary>
		public ServerPartition Partition { get; set; }

		/// <summary>
		/// The "decoded" queue data associated with <see cref="WorkQueueItem"/>
		/// </summary>
		public ReconcileStudyWorkQueueData ReconcileWorkQueueData { get; set; }

		/// <summary>
		/// The <see cref="StudyHistory"/> associated with the <see cref="WorkQueueItem"/>
		/// </summary>
		public StudyHistory History { get; set; }

		/// <summary>
		/// The <see cref="StudyStorageLocation"/> of the resultant study which the images will be reconciled to.
		/// </summary>
		//public StudyStorageLocation DestStorageLocation { get; set; }

		public IList<WorkQueueUid> WorkQueueUidList { get; set; }

		public StudyStorageLocation WorkQueueItemStudyStorage { get; set; }

		#endregion
	}
}