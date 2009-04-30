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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Base class for implementing archives.
	/// </summary>
	/// <remarks>
	/// Archives supported by ImageServer are implemented via the <see cref="ImageServerArchiveExtensionPoint"/>
	/// plugin.  These plugins must implement the <see cref="IImageServerArchivePlugin"/> 
	/// interface.  The ImageServerArchiveBase class implements a base set of methods that
	/// will be used by any archive plugin.
	/// </remarks>
	public abstract class ImageServerArchiveBase : IImageServerArchivePlugin, IDisposable
	{
		protected PartitionArchive _partitionArchive;
		private ServerPartition _serverPartition;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
		private readonly FilesystemSelector _selector;

		/// <summary>
		/// The <see cref="ServerPartition"/> associated with the archive.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
		}

		/// <summary>
		/// A <see cref="FilesystemSelector"/> used to select a filesystem when restoring studies.
		/// </summary>
		public FilesystemSelector Selector
		{
			get { return _selector; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ImageServerArchiveBase()
		{
			_selector = new FilesystemSelector(FilesystemMonitor.Instance);
		}

		public abstract ArchiveTypeEnum ArchiveType { get; }

		/// <summary>
		/// The persistent store.
		/// </summary>
		public IPersistentStore PersistentStore
		{
			get { return _store; }
		}

		public abstract void Start(PartitionArchive archive);
		public abstract void Stop();

		/// <summary>
		/// Get a list of restore candidates for the archive.
		/// </summary>
		/// <remarks>
		/// Note that at the current time only one cadidate is returned at a time.
		/// </remarks>
		/// <returns>A restore candidate.  null will be returned if no candidates exist.</returns>
		public virtual RestoreQueue GetRestoreCandidate()
		{
			RestoreQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServiceTools.ProcessorId;
				parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
				IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

				if (queueItem != null)
					updateContext.Commit();
			}

			return queueItem;
		}

		/// <summary>
		/// Get candidates for archival on the <see cref="PartitionArchive"/>.
		/// </summary>
		/// <returns>A list of archive candidates.  The list will be empty if no candidates exist.</returns>
		public ArchiveQueue GetArchiveCandidate()
		{
			ArchiveQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryArchiveQueueParameters parms = new QueryArchiveQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServiceTools.ProcessorId;

				IQueryArchiveQueue broker = updateContext.GetBroker<IQueryArchiveQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

				if (queueItem != null)
					updateContext.Commit();
			}

			return queueItem;
		}

		/// <summary>
		/// Load the server partition information for the the archive.
		/// </summary>
		public void LoadServerPartition()
		{
			_serverPartition = ServerPartition.Load(_partitionArchive.ServerPartitionKey);
		}

		/// <summary>
		/// Update an <see cref="ArchiveQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		public void UpdateArchiveQueue(ArchiveQueue item, ArchiveQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateArchiveQueueParameters parms = new UpdateArchiveQueueParameters();
				parms.ArchiveQueueKey = item.GetKey();
				parms.ArchiveQueueStatusEnum = status;
				parms.ScheduledTime = scheduledTime;
				parms.StudyStorageKey = item.StudyStorageKey;
				if (!String.IsNullOrEmpty(item.FailureDescription))
					parms.FailureDescription = item.FailureDescription;

				IUpdateArchiveQueue broker = updateContext.GetBroker<IUpdateArchiveQueue>();

				if (broker.Execute(parms))
				{
					updateContext.Commit();
				}
				else 
					Platform.Log(LogLevel.Error, "Unexpected failure updating ArchiveQueue entry {0}", item.GetKey());
			}
		}

		/// <summary>
		/// Update a <see cref="RestoreQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		public void UpdateRestoreQueue(RestoreQueue item, RestoreQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				if (UpdateRestoreQueue(updateContext, item, status, scheduledTime))
					updateContext.Commit();
			}
		}

		/// <summary>
		/// Update a <see cref="RestoreQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		/// <param name="updateContext">The update context</param>
		public bool UpdateRestoreQueue(IUpdateContext updateContext, RestoreQueue item, RestoreQueueStatusEnum status, DateTime scheduledTime)
		{
			UpdateRestoreQueueParameters parms = new UpdateRestoreQueueParameters();
			parms.RestoreQueueKey = item.GetKey();
			parms.RestoreQueueStatusEnum = status;
			parms.ScheduledTime = scheduledTime;
			parms.StudyStorageKey = item.StudyStorageKey;
			if (!String.IsNullOrEmpty(item.FailureDescription))
				parms.FailureDescription = item.FailureDescription;
				
			IUpdateRestoreQueue broker = updateContext.GetBroker<IUpdateRestoreQueue>();

			if (broker.Execute(parms))
			{
				return true;
			}
			
			Platform.Log(LogLevel.Error, "Unexpected failure updating RestoreQueue entry {0}", item.GetKey());
			return false;
		}

		/// <summary>
		/// Dispose the class.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
