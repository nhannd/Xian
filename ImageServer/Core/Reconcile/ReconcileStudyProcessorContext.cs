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