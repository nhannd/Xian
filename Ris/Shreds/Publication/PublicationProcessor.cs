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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
	/// Defines the interface for a publication action, that is, an action to be taken
    /// upon publication of a radiololgy report.
	/// </summary>
	public interface IPublicationAction
	{
		void Execute(PublicationStep step, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class PublicationActionExtensionPoint : ExtensionPoint<IPublicationAction>
	{
	}

    /// <summary>
    /// Processes <see cref="PublicationStep"/>s, performing all <see cref="IPublicationAction"/>s
    /// on each step.
    /// </summary>
    internal class PublicationProcessor : EntityQueueProcessor<PublicationStep>
	{
		private readonly object[] _publicationActions;
		private readonly PublicationShredSettings _settings;

        public PublicationProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
        {
        	_settings = settings;
            _publicationActions = new PublicationActionExtensionPoint().CreateExtensions();
        }

		protected override IList<PublicationStep> GetNextEntityBatch(int batchSize)
		{
			// Get scheduled steps, where the "publishing cool-down" has elapsed
            // eg LastFailureTime is more than a specified number of seconds ago
			PublicationStepSearchCriteria noFailures = GetCriteria();
			noFailures.LastFailureTime.IsNull();
			noFailures.Scheduling.StartTime.SortAsc(0);

			PublicationStepSearchCriteria failures = GetCriteria();
			failures.LastFailureTime.LessThan(Platform.Time.AddSeconds(-_settings.FailedItemRetryDelay));

			PublicationStepSearchCriteria[] criteria = new PublicationStepSearchCriteria[] { noFailures, failures };
			SearchResultPage page = new SearchResultPage(0, batchSize);

			return PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().Find(criteria, page);
		}

		protected override void ActOnItem(PublicationStep item)
		{
			// execute each publication action
			foreach (IPublicationAction action in _publicationActions)
			{
				action.Execute(item, PersistenceScope.CurrentContext);
			}
		}

		protected override void OnItemFailed(PublicationStep item, Exception error)
		{
			// mark item as failed
			item.Fail();
		}

		protected override void OnItemSucceeded(PublicationStep item)
		{
			// all actions succeeded, so mark the publication item as being completed
			item.Complete(item.AssignedStaff);
		}

		private static PublicationStepSearchCriteria GetCriteria()
		{
			PublicationStepSearchCriteria criteria = new PublicationStepSearchCriteria();
			criteria.State.EqualTo(ActivityStatus.SC);
			criteria.Scheduling.Performer.Staff.IsNotNull();
			criteria.Scheduling.StartTime.LessThan(Platform.Time);
			return criteria;
		}
	}
}
