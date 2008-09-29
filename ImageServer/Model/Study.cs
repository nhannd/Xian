using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Study
    {
        static private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        
        /// <summary>
        /// Find a <see cref="Study"/> with the specified study instance uid on the given partition.
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        static public Study Find(String studyInstanceUid, ServerPartition partition)
        {
            IReadContext readContext = _store.OpenReadContext();
            IStudyEntityBroker broker = readContext.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            Study study = broker.FindOne(criteria);
            return study;
        }

    }
}
