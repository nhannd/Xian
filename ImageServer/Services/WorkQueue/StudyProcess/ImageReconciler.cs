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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Reconcile a Dicom image against a study.
    /// </summary>
    /// <remarks>
    /// When an image is reconciled, it may either entered the Study Integrity Queue for user manual intervention
    /// or, if reconciliation has happened before, a <see cref="ReconcileStudy"/> work queue will be spawned to update the information in the image before merging
    /// it into the study.
    /// </remarks>
    class ImageReconciler
    {
        #region Private Members
        private ServerPartition _partition;
        private Study _existingStudy;
        private StudyStorageLocation _existingStudyLocation;
    	private bool _duplicate;

    	#endregion

        #region Public Properties

        /// <summary>
        /// The server partition where the study is located.
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        /// <summary>
        /// The existing <see cref="StudyStorageLocation"/> of the study.
        /// </summary>
        public StudyStorageLocation ExistingStudyLocation
        {
            get { return _existingStudyLocation; }
            set { _existingStudyLocation = value; }
        }

        /// <summary>
        /// The study against which the image will be reconciled.
        /// </summary>
        public Study ExistingStudy
        {
            get { return _existingStudy; }
            set { _existingStudy = value; }
        }

    	public bool Duplicate
    	{
			get { return _duplicate; }
			set { _duplicate = value; }
    	}

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the <see cref="StudyHistory"/> record that matches the information of the specified <see cref="DicomFile"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// The returned list is sorted by insert time. 
        /// </returns>
        private IList<StudyHistory> FindHistory(DicomFile file)
        {
            IList<StudyHistory> histories = StudyHistory.Find(ExistingStudyLocation);

            ImageSetDescriptor fileDesc = ImageSetDescriptor.Parse(file);
            XmlNode node = XmlUtils.Serialize(fileDesc);

#if DEBUG
            // This is important: Make sure the serialize and deserialization are correct. 
            ImageSetDescriptor deserializeDesc = XmlUtils.Deserialize<ImageSetDescriptor>(node);
            Debug.Assert(deserializeDesc.Equals(fileDesc));
#endif

            if (histories == null || histories.Count == 0)
                return null;

            // Find the one with matching demographics
            List<StudyHistory> historyList = CollectionUtils.Cast<StudyHistory>(histories);
            CollectionUtils.Remove(historyList, 
                delegate(StudyHistory history)
                {
                    XmlDocument studyDescriptor = history.StudyData;
                    Debug.Assert(studyDescriptor != null);

                    ImageSetDescriptor desc = ImageSetDescriptor.Parse(studyDescriptor.DocumentElement);

                    if (!desc.Equals(fileDesc))
                        return true; // remove if it's not for this image set
                    else
                        return false;
                });

            CollectionUtils.Sort(historyList,
                                 delegate(StudyHistory history1, StudyHistory history2)
                                     {
                                         return history1.InsertTime.CompareTo(history2.InsertTime);
                                     });

            return historyList;
        }

        /// <summary>
        /// Finds the <see cref="StudyHistory"/> record of the specified type and matching the information of the specified <see cref="DicomFile"/>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IList<StudyHistory> FindHistory(DicomFile file, StudyHistoryTypeEnum type)
        {
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(type, "type");

            IList<StudyHistory> historyList = FindHistory(file);
            if (historyList!=null)
            {
                CollectionUtils.Remove(historyList,
                 delegate(StudyHistory history)
                 {
                     return !history.StudyHistoryTypeEnum.Equals(type);
                 });
            }
            
            return historyList;
        }


        /// <summary>
        /// Returns the path to a directory that can be used for storing images that need to be reconciled
        /// </summary>
        /// <returns></returns>
        private string GetSuggestedTemporaryReconcileFolderPath()
        {
            string path = Path.Combine(ExistingStudyLocation.FilesystemPath, ExistingStudyLocation.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, Guid.NewGuid().ToString());
            return path;

        }

        #endregion

        #region Public Methods
        public void ReconcileImage(DicomFile file)
        {
            Platform.CheckForNullReference(Partition, "Partition");
            Platform.CheckForNullReference(ExistingStudy, "ExistingStudy");
            Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");

            String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            String modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);
            String studyDescription = file.DataSet[DicomTags.StudyDescription].GetString(0, String.Empty);

            ReconcileImageContext reconcileContext = new ReconcileImageContext();
            reconcileContext.Partition = _partition;
            reconcileContext.CurrentStudyLocation = ExistingStudyLocation;
            reconcileContext.File = file;
            reconcileContext.CurrentStudy = ExistingStudy;

            IList<StudyHistory> historyList = FindHistory(file, StudyHistoryTypeEnum.StudyReconciled);

            if (historyList == null || historyList.Count==0 )
            {
                ServerCommandProcessor processor = new ServerCommandProcessor("Schedule new reconciliation");
                MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(reconcileContext, Duplicate);
                InsertReconcileQueueCommand updateQueueCommand = new InsertReconcileQueueCommand(reconcileContext);
                processor.AddCommand(updateQueueCommand);
                processor.AddCommand(moveFileCommand);
                Platform.Log(LogLevel.Info, "Scheduling manual reconciliation. Image contents: Patient={0}, Study={1} ({2})", patientsName, studyDescription, modality);
                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to schedule image reconcilation : {0}", processor.FailureReason));
                }
            }
            else
            {
                reconcileContext.History = historyList[0]; 
                if (reconcileContext.History.DestStudyStorageKey != null)
                {
                    StudyStorage destStorage = StudyStorage.Load(reconcileContext.History.DestStudyStorageKey);
                    Debug.Assert(destStorage != null);

                    reconcileContext.DestinationStudyLocation = StudyStorageLocation.FindStorageLocations(destStorage)[0];
                    Debug.Assert(reconcileContext.DestinationStudyLocation != null);

                    Study destStudy = Study.Find(destStorage.StudyInstanceUid, _partition); // note: assuming destination partition is the same as the current one
                    Debug.Assert(destStudy != null);

                    Platform.Log(LogLevel.Info, "Auto reconciliation: Patient={0}, Study={1} ({2}). Target Study: StorageGUID={3}, UID={4}, A#={5}.  Referenced history: GUID={6}",
                            patientsName, studyDescription, modality, reconcileContext.DestinationStudyLocation.GetKey(), destStudy.StudyInstanceUid, destStudy.AccessionNumber, reconcileContext.History.GetKey());

                }
                else
                {
                    Platform.Log(LogLevel.Info, "Auto reconciliation: Patient={0}, Study={1} ({2}). Referenced history: GUID={3}",
                           patientsName, studyDescription, modality, reconcileContext.History.GetKey());

                }

                // Insert 'ReconcileStudy' in the work queue
                reconcileContext.StoragePath = GetSuggestedTemporaryReconcileFolderPath(); // since we no longer have the record in the Study Integrity Queue
                ServerCommandProcessor processor = new ServerCommandProcessor("Schedule ReconcileStudy request based on histrory");
                InsertReconcileStudyCommand insertCommand = new InsertReconcileStudyCommand(reconcileContext);
                
                MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(reconcileContext, Duplicate);
                processor.AddCommand(insertCommand);
                processor.AddCommand(moveFileCommand);

                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to create ReconcileStudy request: {0}", processor.FailureReason));
                }

                Debug.Assert(insertCommand.ReconcileStudyWorkQueueItem != null);
            }
        }

        #endregion

    }
}
