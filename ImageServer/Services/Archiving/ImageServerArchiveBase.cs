#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Base class for implementing archives.
	/// </summary>
	public abstract class ImageServerArchiveBase : IImageServerArchivePlugin
	{
		protected PartitionArchive _partitionArchive;
		private ServerPartition _serverPartition;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
		}
		public abstract ArchiveTypeEnum ArchiveType { get; }

		public abstract void Start(PartitionArchive archive);
		public abstract void Stop();

		public IList<RestoreQueue> GetRestoreCandidate()
		{
			IList<RestoreQueue> list;

			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServiceTools.ProcessorId;

				IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

				list = broker.Execute(parms);

				if (list.Count > 0)
					updateContext.Commit();
			}

			return list;
		}

		/// <summary>
		/// Get candidates for archival on the <see cref="PartitionArchive"/>.
		/// </summary>
		/// <returns></returns>
		public IList<ArchiveQueue> GetArchiveCandidates()
		{
			IList<ArchiveQueue> list;

			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryArchiveQueueParameters parms = new QueryArchiveQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServiceTools.ProcessorId;

				IQueryArchiveQueue broker = updateContext.GetBroker<IQueryArchiveQueue>();

				list = broker.Execute(parms);

				if (list.Count > 0)
					updateContext.Commit();
			}

			return list;
		}

		/// <summary>
		/// Load the server partition information for the 
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
			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateArchiveQueueParameters parms = new UpdateArchiveQueueParameters();
				parms.ArchiveQueueKey = item.GetKey();
				parms.ArchiveQueueStatusEnum = status;
				parms.ScheduledTime = scheduledTime;
				parms.StudyStorageKey = item.StudyStorageKey;

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
		/// Update an <see cref="RestoreQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		public void UpdateRestoreQueue(RestoreQueue item, RestoreQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateRestoreQueueParameters parms = new UpdateRestoreQueueParameters();
				parms.RestoreQueueKey = item.GetKey();
				parms.RestoreQueueStatusEnum = status;
				parms.ScheduledTime = scheduledTime;
				parms.StudyStorageKey = item.StudyStorageKey;

				IUpdateRestoreQueue broker = updateContext.GetBroker<IUpdateRestoreQueue>();

				if (broker.Execute(parms))
				{
					updateContext.Commit();
				}
				else
					Platform.Log(LogLevel.Error, "Unexpected failure updating RestoreQueue entry {0}", item.GetKey());
			}
		}
	}
}
