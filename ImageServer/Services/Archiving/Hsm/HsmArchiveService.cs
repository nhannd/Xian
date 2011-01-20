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
	/// Service thread for handling archivals.
	/// </summary>
	public class HsmArchiveService : ThreadedService
	{
		private readonly HsmArchive _hsmArchive;
		private readonly ItemProcessingThreadPool<ArchiveQueue> _threadPool;

		public HsmArchiveService(string name, HsmArchive hsmArchive) : base(name)
		{
			_hsmArchive = hsmArchive;
			_threadPool = new ItemProcessingThreadPool<ArchiveQueue>(HsmSettings.Default.ArchiveThreadCount);
			_threadPool.ThreadPoolName = "HsmArchive Pool";
		}

		protected override void Initialize()
		{
			_hsmArchive.ResetFailedArchiveQueueItems();

			// Start the thread pool
			if (!_threadPool.Active)
				_threadPool.Start();
		}

		/// <summary>
		/// Execute the service.
		/// </summary>
		protected override void Run()
		{
			while (true)
			{
				if ((_threadPool.QueueCount + _threadPool.ActiveCount) < _threadPool.Concurrency)
				{
					try
					{
						ArchiveQueue queueItem = _hsmArchive.GetArchiveCandidate();

						if (queueItem != null)
						{
							HsmStudyArchive archiver = new HsmStudyArchive(_hsmArchive);
							_threadPool.Enqueue(queueItem, archiver.Run);
						}
						else if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error,e,"Unexpected exception when querying for archive candidates, rescheduling.");
						if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
				}
				else
				{
					if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
					{
						Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Stop the service.
		/// </summary>
		protected override void Stop()
		{
			_threadPool.Stop(true);
		}
	}
}
