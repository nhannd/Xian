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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.Shreds
{
	/// <summary>
	/// Shreds namespace manager of processing threads for the WorkQueue.
	/// </summary>
	/// <remarks>
	/// The service manager is currently setup to create two WorkQueue processors,
	/// a primary processor and secondary processor.  Through configuration, the specific types
	/// of WorkQueue entries supported by each processor can be set.  By default, the primary
	/// processor supports high priority queue types for processing studies, editing studies, and 
	/// doing moves/auto-routes.  The secondary processor will process any queue entries.
	/// </remarks>
	public class WorkQueueServerManager : ThreadedService
	{
		#region Private Members
		private static WorkQueueServerManager _instance;
		private WorkQueueProcessor _theProcessor;
		private readonly int _threadCount;
		private static bool _reset = false;
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private WorkQueueServerManager(string name) : base(name)
		{
			_threadCount = WorkQueueSettings.Instance.WorkQueueThreadCount;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static WorkQueueServerManager PrimaryInstance
		{
			get
			{
				if (_instance == null)
				{
					if (!_reset)
					{
						_reset = true;
						// Reset any queue items related to this system that are in a "In Progress" state.
						try
						{
							ResetFailedItems();
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Fatal, e,
										 "Unable to reset WorkQueue items on startup.  There may be WorkQueue items orphaned in the queue.");
						}
					}
					_instance =
						new WorkQueueServerManager("WorkQueue");
				}

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

        
		#region Protected Methods
		/// <summary>
		/// Reset queue items that were unadvertly left in "in progress" state by previous run. 
		/// </summary>
		public static void ResetFailedItems()
		{
			WorkQueueSettings settings = WorkQueueSettings.Instance;

			WorkQueueStatusEnum pending = WorkQueueStatusEnum.Pending;
			WorkQueueStatusEnum failed = WorkQueueStatusEnum.Failed;

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueReset reset = ctx.GetBroker<IWorkQueueReset>();
				WorkQueueResetParameters parms = new WorkQueueResetParameters();
				parms.ProcessorID = ServerPlatform.ProcessorId;

				// reschedule to start again now
				parms.RescheduleTime = Platform.Time;
				// retry will expire X minutes from now (so other process MAY NOT remove them)
				parms.RetryExpirationTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);

				// if an entry has been retried more than WorkQueueMaxFailureCount, it should be failed
				parms.MaxFailureCount = settings.WorkQueueMaxFailureCount;
				// failed item expires now (so other process can remove them if desired)
				parms.FailedExpirationTime = Platform.Time;

				IList<Model.WorkQueue> modifiedList = reset.Find(parms);

				if (modifiedList != null)
				{
					// output the list of items that have been reset
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(pending))
							Platform.Log(LogLevel.Info, "Cleanup: Reset Queue Item : {0} --> Status={1} Scheduled={2} ExpirationTime={3}",
											queueItem.GetKey().Key,
											queueItem.WorkQueueStatusEnum,
											queueItem.ScheduledTime,
											queueItem.ExpirationTime);
					}

					// output the list of items that have been failed because it exceeds the max retry count
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(failed))
							Platform.Log(LogLevel.Info, "Cleanup: Fail Queue Item  : {0} : FailureCount={1} ExpirationTime={2}",
											queueItem.GetKey().Key,
											queueItem.FailureCount,
											queueItem.ExpirationTime);
					}
				}

				ctx.Commit();
			}
		}

		protected override void Initialize()
		{
			if (_theProcessor == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failng first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
					readContext.Dispose();
				}

				_theProcessor = new WorkQueueProcessor(_threadCount, ThreadStop, Name); 
			
			}				
		}

		protected override void Run()
		{
			_theProcessor.Run();
		}

		protected override void Stop()
		{
			if (_theProcessor != null)
			{
				_theProcessor.Stop();
				_theProcessor = null;
			}
		}
		#endregion
	}
}