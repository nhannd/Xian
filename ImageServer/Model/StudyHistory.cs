using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class StudyHistory
    {
        static private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

        /// <summary>
        /// Finds a list of <see cref="StudyHistory"/> records for the specified <see cref="StudyStorageLocation"/>.
        /// </summary>
        /// <param name="storageLocation"></param>
        /// <returns></returns>
        static public IList<StudyHistory> Find(StudyStorageLocation storageLocation)
        {
            using(IReadContext readContext = _store.OpenReadContext())
            {
                IStudyHistoryEntityBroker broker = readContext.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistorySelectCriteria criteria = new StudyHistorySelectCriteria();
                criteria.StudyStorageKey.EqualTo(storageLocation.GetKey());
                criteria.StudyHistoryTypeEnum.EqualTo(StudyHistoryTypeEnum.StudyReconciled);
                IList<StudyHistory> historyList = broker.Find(criteria);
                return historyList;
            }
            
        }

    }
}
