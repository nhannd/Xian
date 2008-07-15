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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	public class HsmArchiveService : ThreadedService
	{
		private PartitionArchive _archive;
		private HsmArchive _hsmArchive;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
		private readonly ItemProcessingThreadPool<ArchiveQueue> _threadPool;

		public HsmArchiveService(string name, PartitionArchive archive, HsmArchive hsmArchive) : base(name)
		{
			_archive = archive;
			_hsmArchive = hsmArchive;
			_threadPool = new ItemProcessingThreadPool<ArchiveQueue>(HsmSettings.Default.ArchiveThreadCount);
			_threadPool.ThreadPoolName = "HsmArchive Pool";
		}

		protected override void Initialize()
		{
			// Start the thread pool
			if (!_threadPool.Active)
				_threadPool.Start();
		}

		protected override void Run()
		{
			while (true)
			{
				bool foundResult = false;

				if ((_threadPool.QueueCount + _threadPool.ActiveCount) < _threadPool.Concurrency)
				{
					IList<ArchiveQueue> list;

					using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
					{
						QueryArchiveQueueParameters parms = new QueryArchiveQueueParameters();

						parms.PartitionArchiveKey = _archive.GetKey();
						parms.ProcessorId = ServiceTools.ProcessorId;

						IQueryArchiveQueue broker = updateContext.GetBroker<IQueryArchiveQueue>();

						list = broker.Execute(parms);

						updateContext.Commit();
					}

					if (list.Count > 0)
						foundResult = true;

					foreach (ArchiveQueue queueListItem in list)
					{
						HsmStudyArchive archiver = new HsmStudyArchive(_archive);
						_threadPool.Enqueue(queueListItem, archiver.Run);
					}

					if (!foundResult)
						if (CheckStop(100))
							return;
				}
				else
				{
					if (CheckStop(5000))
					{
						return;
					}
				}
			}
		}

		protected override void Stop()
		{
		

		}
	}
}
