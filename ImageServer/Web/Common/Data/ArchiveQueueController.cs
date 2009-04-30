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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class ArchiveQueueController
	{
        private readonly ArchiveQueueAdaptor _adaptor = new ArchiveQueueAdaptor();


		/// <summary>
		/// Gets a list of <see cref="ArchiveQueue"/> items with specified criteria
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList<ArchiveQueue> FindArchiveQueue(WebQueryArchiveQueueParameters parameters)
		{
			try
			{
				IList<ArchiveQueue> list;

				IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

                IWebQueryArchiveQueue broker = HttpContextData.Current.ReadContext.GetBroker<IWebQueryArchiveQueue>();
				list = broker.Find(parameters);

				return list;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "FindArchiveQueue failed", e);
				return new List<ArchiveQueue>();
			}
		}

        public bool DeleteArchiveQueueItem(ArchiveQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

		public bool ResetArchiveQueueItem(IList<ArchiveQueue> items, DateTime time)
		{
			if (items == null || items.Count == 0)
				return false;

			ArchiveQueueUpdateColumns columns = new ArchiveQueueUpdateColumns();
			columns.ArchiveQueueStatusEnum = ArchiveQueueStatusEnum.Pending;
			columns.ProcessorId = "";
			columns.ScheduledTime = time;

			bool result = true;
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IArchiveQueueEntityBroker archiveQueueBroker = ctx.GetBroker<IArchiveQueueEntityBroker>();
				
				foreach (ArchiveQueue item in items)
				{
					// Only do an update if its a failed status currently
					ArchiveQueueSelectCriteria criteria = new ArchiveQueueSelectCriteria();
					criteria.ArchiveQueueStatusEnum.EqualTo(ArchiveQueueStatusEnum.Failed);
					criteria.StudyStorageKey.EqualTo(item.StudyStorageKey);

					if (!archiveQueueBroker.Update(criteria, columns))
					{
						result = false;
						break;
					}
				}

				if (result)
					ctx.Commit();
			}

			return result;
		}
	}
}
