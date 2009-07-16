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
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public partial class StudyStorage
    {
    	#region Private Fields
        private Study _study;
        private ServerPartition _partition;
		#endregion
		
		#region Public Properties
        

        public ServerPartition ServerPartition
        {
            get
            {
                if (_partition==null)
                {
                    _partition = ServerPartition.Load(this.ServerPartitionKey);
                }
                return _partition;
            }
        }

        public Study Study
        {
            get
            {
                if (_study==null)
                {
                    lock (SyncRoot)
                    {
                        // TODO: Use ExecutionContext to re-use db connection if possible
                        // This however requires breaking the Common --> Model dependency.
                        using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            _study = LoadStudy(readContext);
                        }
                    }
                }

                return _study;
            }
        }

        #endregion
		
		public Study LoadStudy(IPersistenceContext context)
        {
            return Study.Find(context, this.StudyInstanceUid, this.ServerPartition);
        }
		
        public void Archive(IUpdateContext context)
        {
            IInsertArchiveQueue insertArchiveQueueBroker = context.GetBroker<IInsertArchiveQueue>();
            InsertArchiveQueueParameters parms = new InsertArchiveQueueParameters();
            parms.ServerPartitionKey = this.ServerPartitionKey;
            parms.StudyStorageKey = this.GetKey();
            if (!insertArchiveQueueBroker.Execute(parms))
            {
                throw new ApplicationException("Unable to schedule study archive");
            }
        }

		public static StudyStorage Load(IPersistenceContext read, ServerEntityKey partitionKey, string studyInstanceUid)
		{
	        IStudyStorageEntityBroker broker = read.GetBroker<IStudyStorageEntityBroker>();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			criteria.ServerPartitionKey.EqualTo(partitionKey);
            StudyStorage theObject = broker.FindOne(criteria);
            return theObject;        
		}

		public static StudyStorage Load(ServerEntityKey partitionKey, string studyInstanceUid)
		{
			using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				return Load(context, partitionKey, studyInstanceUid);
			}
		}
    }
}
