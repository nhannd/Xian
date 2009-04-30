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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Helper class for manipulating the Image Availability Work Queue.
	/// </summary>
	public static class ImageAvailabilityWorkQueue
	{
		private const string ProcedureOIDKey = "ProcedureOID";
		public const string WorkQueueItemType = "ImageAvailability";

		/// <summary>
		/// Creates a new work item.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="expirationTime"></param>
		/// <returns></returns>
		public static WorkQueueItem CreateWorkQueueItem(Procedure p, TimeSpan expirationTime)
		{
			WorkQueueItem item = new WorkQueueItem(WorkQueueItemType);
			item.ExpirationTime = Platform.Time.Add(expirationTime);
			item.ExtendedProperties.Add(ProcedureOIDKey, p.GetRef().Serialize());

			return item;
		}

		/// <summary>
		/// Polls the queue for pending items.
		/// </summary>
		/// <param name="batchSize"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IList<WorkQueueItem> GetPendingItems(int batchSize, IPersistenceContext context)
		{
			return context.GetBroker<IWorkQueueItemBroker>().GetPendingItems(WorkQueueItemType, batchSize);
		}

		/// <summary>
		/// Gets the procedure associated with the specified work item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static Procedure GetProcedure(WorkQueueItem item, IPersistenceContext context)
		{
			EntityRef procedureRef = new EntityRef(item.ExtendedProperties[ProcedureOIDKey]);
			return context.Load<Procedure>(procedureRef, EntityLoadFlags.Proxy);
		}
	}
}
