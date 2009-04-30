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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes in-progress procedures, and schedules corresponding Image Availability work queue items.
	/// </summary>
	public class ImageAvailabilityProcedureProcessor : EntityQueueProcessor<Procedure>
	{
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
		}

		protected override IList<Procedure> GetNextEntityBatch(int batchSize)
		{
			// Find a list of procedures that match the criteria
			ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
			criteria.Status.EqualTo(ProcedureStatus.IP);
			criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

			SearchResultPage page = new SearchResultPage(0, batchSize);
			return PersistenceScope.CurrentContext.GetBroker<IProcedureBroker>().Find(criteria, page);
		}

		protected override void ActOnItem(Procedure procedure)
		{
			// create the workqueue item
			TimeSpan expirationTime = TimeSpan.FromHours(_settings.ExpirationTime);
			WorkQueueItem item = ImageAvailabilityWorkQueue.CreateWorkQueueItem(procedure, expirationTime);
			PersistenceScope.CurrentContext.Lock(item, DirtyState.New);
		}

		protected override void OnItemSucceeded(Procedure item)
		{
			// Set this to Not Available so the worklist item doesn't get created again for this procedure
			item.ImageAvailability = Healthcare.ImageAvailability.N;
		}

		protected override void OnItemFailed(Procedure item, Exception error)
		{
			// do nothing
		}
	}
}
