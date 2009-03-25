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
        public Study Study
        {
            get
            {
                if (_study==null)
                {
                    _study = Study.Find(this.StudyInstanceUid, this.ServerPartition);
                }
                return _study;
            }
        }

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
		
		#endregion
		
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
