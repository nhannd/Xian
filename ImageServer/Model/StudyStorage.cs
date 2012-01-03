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
                    _partition = ServerPartition.Load(ServerPartitionKey);
                }
                return _partition;
            }
        }

        #endregion
		
		public Study LoadStudy(IPersistenceContext context)
        {
            return Study.Find(context, StudyInstanceUid, ServerPartition);
        }

        public Study GetStudy()
        {
            if (_study == null)
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
		
        public void Archive(IUpdateContext context)
        {
            var insertArchiveQueueBroker = context.GetBroker<IInsertArchiveQueue>();
            var parms = new InsertArchiveQueueParameters
                            {
                                ServerPartitionKey = ServerPartitionKey, 
                                StudyStorageKey = Key
                            };
            if (!insertArchiveQueueBroker.Execute(parms))
            {
                throw new ApplicationException("Unable to schedule study archive");
            }
        }

		/// <summary>
		/// Insert a request to restore the specified <seealso cref="StudyStorage"/>
		/// </summary>
		/// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
		public RestoreQueue InsertRestoreRequest()
		{
			// TODO:
			// Check the stored procedure to see if it will insert another request if one already exists

			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				var broker = updateContext.GetBroker<IInsertRestoreQueue>();
				var parms = new InsertRestoreQueueParameters { StudyStorageKey = Key };

				RestoreQueue queue = broker.FindOne(parms);

				if (queue == null)
				{
					Platform.Log(LogLevel.Error, "Unable to request restore for study {0}", StudyInstanceUid);
					return null;
				}

				updateContext.Commit();
				Platform.Log(LogLevel.Info, "Restore requested for study {0}", StudyInstanceUid);
				return queue;
			}
		}

		public static StudyStorage Load(IPersistenceContext read, ServerEntityKey partitionKey, string studyInstanceUid)
		{
	        var broker = read.GetBroker<IStudyStorageEntityBroker>();
			var criteria = new StudyStorageSelectCriteria();
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
