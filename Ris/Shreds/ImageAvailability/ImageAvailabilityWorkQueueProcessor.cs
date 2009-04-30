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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes the Image Availability work queue.
	/// </summary>
	public class ImageAvailabilityWorkQueueProcessor : WorkQueueProcessor
	{
		private readonly IImageAvailabilityStrategy _imageAvailabilityStrategy;
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityWorkQueueProcessor(ImageAvailabilityShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
			try
			{
				_imageAvailabilityStrategy = (IImageAvailabilityStrategy)(new ImageAvailabilityStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_imageAvailabilityStrategy = new DefaultImageAvailabilityStrategy();
			}
		}

		protected override string WorkQueueItemType
		{
			get { return ImageAvailabilityWorkQueue.WorkQueueItemType; }
		}

		protected override void ActOnItem(WorkQueueItem item)
		{
			Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, PersistenceScope.CurrentContext);
			procedure.ImageAvailability = _imageAvailabilityStrategy.ComputeProcedureImageAvailability(procedure, PersistenceScope.CurrentContext);
		}

		protected override void OnItemSucceeded(WorkQueueItem item)
		{
			// this method is overridden because image availability work items are never considered complete until they expire

			Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, PersistenceScope.CurrentContext);
			DateTime nextPollTime = Platform.Time.Add(GetPollingInterval(procedure.ImageAvailability));
			if(nextPollTime < item.ExpirationTime)
			{
				item.Reschedule(nextPollTime);
			}
			else
			{
				base.OnItemSucceeded(item);
			}
		}

		protected override bool ShouldRetry(WorkQueueItem item, Exception error, out DateTime retryTime)
		{
			// retry unless expired
			retryTime = Platform.Time.AddSeconds(_settings.PollingIntervalForError);
			return (retryTime < item.ExpirationTime);
		}

		private TimeSpan GetPollingInterval(Healthcare.ImageAvailability imageAvailability)
		{
			switch (imageAvailability)
			{
				case Healthcare.ImageAvailability.N:
					return TimeSpan.FromSeconds(_settings.PollingIntervalForIndeterminate);
				case Healthcare.ImageAvailability.Z:
					return TimeSpan.FromSeconds(_settings.PollingIntervalForZero);
				case Healthcare.ImageAvailability.P:
					return TimeSpan.FromSeconds(_settings.PollingIntervalForPartial);
				case Healthcare.ImageAvailability.C:
					return TimeSpan.FromSeconds(_settings.PollingIntervalForComplete);
				default:
					// ImageAvailability.X should never get pass into this method
					throw new NotImplementedException();
			}
		}
	}
}
