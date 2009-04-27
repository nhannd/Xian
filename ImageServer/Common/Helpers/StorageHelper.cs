using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    
    public static class StorageHelper
    {
        /// <summary>
        /// Returns the name of the directory in the filesytem
        /// where the study with the specified information will be stored.
        /// </summary>
        /// <returns></returns>
        /// 
        public static string ResolveStorageFolder(
            ServerPartition partition, 
            string studyInstanceUid, 
            string studyDate,
            IPersistenceContext persistenceContext,
            bool checkExisting)
        {
            string folder;

            if (checkExisting)
            {
                StudyStorage storage = StudyHelper.FindStorage(persistenceContext, studyInstanceUid, partition);
                if (storage != null)
                {
                    folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
                                    ? storage.InsertTime.ToString("yyyyMMdd")
                                    : String.IsNullOrEmpty(studyDate)
                                          ? ImageServerCommonConfiguration.DefaultStudyRootFolder
                                          : studyDate;
                    return folder;
                }
            }

            folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
                                ? Platform.Time.ToString("yyyyMMdd")
                                : String.IsNullOrEmpty(studyDate)
                                      ? ImageServerCommonConfiguration.DefaultStudyRootFolder
                                      : studyDate;

            return folder;

        }


        /// <summary>
        /// Checks for a storage location for the study in the database, and creates a new location
        /// in the database if it doesn't exist.
        /// </summary>
        /// <param name="message">The DICOM message to create the storage location for.</param>
        /// <param name="partition">The partition where the study is being sent to</param>
        /// <returns>A <see cref="StudyStorageLocation"/> instance.</returns>
        static public StudyStorageLocation GetStudyStorageLocation(DicomMessageBase message, ServerPartition partition)
        {
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            String studyDate = message.DataSet[DicomTags.StudyDate].GetString(0, "");

            FilesystemSelector selector = new FilesystemSelector(FilesystemMonitor.Instance);
            ServerFilesystemInfo filesystem = selector.SelectFilesystem(message);
            if (filesystem == null)
            {
                Platform.Log(LogLevel.Error, "Unable to select location for storing study.");

                return null;
            }


            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IQueryStudyStorageLocation locQuery = updateContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
                locParms.StudyInstanceUid = studyInstanceUid;
                locParms.ServerPartitionKey = partition.GetKey();
                IList<StudyStorageLocation> studyLocationList = locQuery.Find(locParms);

                if (studyLocationList.Count == 0)
                {
                    StudyStorage storage = StudyHelper.FindStorage(updateContext, studyInstanceUid, partition);

                    if (storage != null)
                    {
                        Platform.Log(LogLevel.Warn, "Study in {0} state.  Rejecting image.", storage.StudyStatusEnum.Description);
                        return null;
                    }

                    IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
                    InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
                    insertParms.ServerPartitionKey = partition.GetKey();
                    insertParms.StudyInstanceUid = studyInstanceUid;
                    
                    insertParms.Folder = ResolveStorageFolder(partition, studyInstanceUid, studyDate, updateContext, false /* set to false for optimization because we are sure it's not in the system */);
                    insertParms.FilesystemKey = filesystem.Filesystem.GetKey();
                    insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

                    if (message.TransferSyntax.LosslessCompressed)
                    {
                        insertParms.TransferSyntaxUid = message.TransferSyntax.UidString;
                        insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                    }
                    else if (message.TransferSyntax.LossyCompressed)
                    {
                        insertParms.TransferSyntaxUid = message.TransferSyntax.UidString;
                        insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
                    }
                    else
                    {
                        insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
                        insertParms.StudyStatusEnum = StudyStatusEnum.Online;
                    }

                    studyLocationList = locInsert.Find(insertParms);

                    updateContext.Commit();
                }
                else
                {
                    if (!FilesystemMonitor.Instance.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
                    {
                        Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
                                     studyInstanceUid, partition.Description);
                        return null;
                    }
                }

                //TODO:  Do we need to do something to identify a primary storage location?
                // Also, should the above check for writeable location check the other availab
                return studyLocationList[0];
            }
        }

    }

}
