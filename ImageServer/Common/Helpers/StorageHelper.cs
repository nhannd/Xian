using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    
    public static class StorageHelper
    {
        /// <summary>
        /// Encapsulate the arguments used for <see cref="StorageHelper"/>.
        /// </summary>
        public interface DataContext
        {
            IPersistenceContext PersistenceContext { get; }
            string StudyInstanceUid { get;}
            ServerPartition Partition { get; }
            string GetDicomValue(uint tag);
        }

        /// <summary>
        /// Returns the name of the directory in the filesytem
        /// where the study referenced by the specified <see cref="DicomMessage"></see> will be stored.
        /// </summary>
        /// <param name="context">The data context of the</param>
        /// <param name="checkExisting"></param>
        /// <returns></returns>
        /// 
        public static string ResolveStorageFolder(
                    DataContext context, 
                    bool checkExisting)
        {
            string folder;

            Platform.CheckForNullReference(context, "context");
            Platform.CheckForNullReference(context.Partition, "context.Partition");
            Platform.CheckForEmptyString(context.StudyInstanceUid, "context.StudyInstanceUid");

            string studyDate = context.GetDicomValue(DicomTags.StudyDate);

            if (checkExisting)
            {
                Platform.CheckForNullReference(context.PersistenceContext, "context.PersistenceContext");
                StudyStorage storage = StudyHelper.FindStorage(context.PersistenceContext, context.StudyInstanceUid, context.Partition);
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

    }

}
