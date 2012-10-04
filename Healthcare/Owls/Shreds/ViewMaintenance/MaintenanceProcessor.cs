#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls.Shreds.ViewMaintenance
{
	class MaintenanceProcessor : QueueProcessor
	{
		private const int SnoozeIntervalInMilliseconds = 100;

		private readonly int _batchSize;
		private readonly TimeSpan _longSleepTime;
		private readonly TimeSpan _shortSleepTime = TimeSpan.FromSeconds(1);
		private readonly IViewShrinker _viewShrinker;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="viewShrinker"></param>
		/// <param name="batchSize">Max number of items to pull off queue for processing.</param>
		/// <param name="sleepTime"></param>
		public MaintenanceProcessor(IViewShrinker viewShrinker, int batchSize, TimeSpan sleepTime)
		{
			_viewShrinker = viewShrinker;
			_batchSize = batchSize;
			_longSleepTime = sleepTime;
		}

		public override string Name
		{
			get
			{
				// for now, just return the shred name
				return SR.MaintenanceShredName;
			}
		}

		protected override void RunCore()
		{
			while (!StopRequested)
			{
				var deletedCount = 0;
				try
				{
					using(var scope = new PersistenceScope(PersistenceContextType.Update))
					{
						((IUpdateContext) scope.Context).ChangeSetRecorder.OperationName = this.GetType().FullName;
						deletedCount = _viewShrinker.DeleteItems((IUpdateContext)scope.Context, _batchSize);

						// bug #7144: if no items were actually deleted, allow the transaction to be rolled back,
						// to avoid generating unnecessary audit messages
						if(deletedCount > 0)
							scope.Complete();
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}

				// if less than max number of items were deleted, can afford to sleep for a long time
				// otherwise sleep for a short time, just so we don't create a DoS situation on the database server
				Sleep(deletedCount < _batchSize ? _longSleepTime : _shortSleepTime);
			}
		}

		private void Sleep(TimeSpan sleepTime)
		{
			// sleep for the total sleep time, unless stop requested
			for (var i = 0; i < sleepTime.TotalMilliseconds
				&& !StopRequested; i += SnoozeIntervalInMilliseconds)
			{
				Thread.Sleep(SnoozeIntervalInMilliseconds);
			}
		}
	}
}
