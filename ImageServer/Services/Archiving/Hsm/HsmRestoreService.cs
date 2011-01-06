#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Service thread for handling restore requests for <see cref="HsmArchive"/>s.
	/// </summary>
	public class HsmRestoreService : ThreadedService
	{
		private readonly HsmArchive _hsmArchive;
		private readonly ItemProcessingThreadPool<RestoreQueue> _threadPool;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the service.</param>
		/// <param name="hsmArchive">The <see cref="HsmArchive"/> for which to do restores. </param>
		public HsmRestoreService(string name, HsmArchive hsmArchive)
			: base(name)
		{
			_hsmArchive = hsmArchive;

			_threadPool = new ItemProcessingThreadPool<RestoreQueue>(HsmSettings.Default.RestoreThreadCount);
			_threadPool.ThreadPoolName = "HsmRestore Pool";
		}

		/// <summary>
		/// Initialize the service.
		/// </summary>
		protected override void Initialize()
		{
			_hsmArchive.ResetFailedRestoreQueueItems();

			// Start the thread pool
			if (!_threadPool.Active)
				_threadPool.Start();
		}

		/// <summary>
		/// Run the service.
		/// </summary>
		protected override void Run()
		{

			while (true)
			{
				if ((_threadPool.QueueCount + _threadPool.ActiveCount) < _threadPool.Concurrency)
				{
					try
					{
						RestoreQueue queueItem = _hsmArchive.GetRestoreCandidate();

						if (queueItem != null)
						{
							HsmStudyRestore archiver = new HsmStudyRestore(_hsmArchive);
							_threadPool.Enqueue(queueItem, archiver.Run);
						}
						else if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} restore service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e, "Unexpected exception when querying for restore candidates.  Rescheduling.");
						if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} restore service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
				}
				else
				{
					if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
					{
						Platform.Log(LogLevel.Info, "Shutting down {0} restore service.", _hsmArchive.PartitionArchive.Description);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Stop the service thread.
		/// </summary>
		protected override void Stop()
		{
			_threadPool.Stop(true);
		}
	}
}
