using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common.Helpers
{

    /// <summary>
    /// Helper class for handling studies
    /// </summary>
    public static class StudyHelper
    {
        /// <summary>
        /// Retrieves the <see cref="StudyStorage"/> record for a specified study.
        /// </summary>
        /// <param name="context">The persistence context used for database I/O</param>
        /// <param name="studyInstanceUid">The study instance uid of the study</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study is located</param>
        /// <returns></returns>
        public static StudyStorage FindStorage(IPersistenceContext context, string studyInstanceUid, ServerPartition partition)
        {
            Platform.CheckForNullReference(context, "context");

            IStudyStorageEntityBroker broker = context.GetBroker<IStudyStorageEntityBroker>();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());

            return broker.FindOne(criteria);
            
        }

        /// <summary>
        /// Verifies the contents of a <see cref="DicomMessageBase"/> against a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        static public DifferenceCollection Compare(DicomMessageBase message, StudyStorageLocation studyStorage)
        {
            StudyComparer comparer = new StudyComparer();
            return comparer.Compare(message, studyStorage.Study, studyStorage.ServerPartition.GetComparisonOptions());
        }
    }
}
