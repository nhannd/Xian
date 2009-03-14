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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Helpers;
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
    	private string _sopInstanceUid;
        private IList<StudyHistory> _historyList;
        private ReconcileImageContext _reconcileContext;

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

            historyList = CollectionUtils.Sort(historyList,
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


        private bool TryAutoCorrection(DicomMessageBase message)
        {
            Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");

            DifferenceCollection list = StudyHelper.Compare(message, ExistingStudyLocation);

            if (list.Count == 1)
            {
                ComparisionDifference different = list[0];
                if (different.DicomTag.TagValue == DicomTags.PatientsName)
                {
                    if (DifferentOnlyByCarets(different.ExpectValue, different.RealValue))
                    {
                        AutoCorrectPatientsName(message, StudyReconcileAction.Merge);
                        return true;
                    }
                }
            }

            return false;
        }

        static bool DifferentOnlyByCarets(string s1, string s2)
        {
            s1 = DicomNameUtils.Normalize(s1,DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);
            s2 = DicomNameUtils.Normalize(s2, DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);
            
            if (s1.Length != s2.Length) return false;

            s1 = s1.ToUpper();
            s2 = s2.ToUpper();

            for (int i = 0; i < s1.Length; i++ )
            {
                if (s1[i]!=s2[i])
                {
                    if (s1[i] == '^' && s2[i] != ' ') return false;
                    if (s2[i] == '^' && s1[i] != ' ') return false;
                }
            }
            return true;
        }

        private void AutoCorrectPatientsName(DicomMessageBase message, StudyReconcileAction method)
        {
            Platform.Log(LogLevel.Info, "Scheduling auto reconciliation to correct patient name...");
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule ReconcileStudy request"))
            {
                _reconcileContext.StoragePath = GetSuggestedTemporaryReconcileFolderPath();
                
                switch(method)
                {
                    case StudyReconcileAction.Merge:
                        {
                            processor.AddCommand(new InsertMergeToExistingStudyHistoryCommand(_reconcileContext));
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
                
                InsertReconcileStudyCommand insertReconcileStudyCommand = new InsertReconcileStudyCommand(_reconcileContext);
                MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);

                processor.AddCommand(insertReconcileStudyCommand);
                processor.AddCommand(moveFileCommand);
                
                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to create ReconcileStudy request: {0}", processor.FailureReason));
                }
                Platform.Log(LogLevel.Info, "SOP {0} has been scheduled for auto reconciliation.", _sopInstanceUid);
            }
        }

        #endregion



        #region Public Methods
        public void ReconcileImage(DicomFile file, bool isDuplicate)
        {
            Platform.CheckForNullReference(Partition, "Partition");
            Platform.CheckForNullReference(ExistingStudy, "ExistingStudy");
            Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");

            _sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            
            //TODO: optimization: use previously loaded history list if possible
            _historyList = FindHistory(file, StudyHistoryTypeEnum.StudyReconciled);

            _reconcileContext = new ReconcileImageContext();
            _reconcileContext.Partition = _partition;
            _reconcileContext.CurrentStudyLocation = ExistingStudyLocation;
            _reconcileContext.File = file;
            _reconcileContext.IsDuplicate = isDuplicate;
            _reconcileContext.CurrentStudy = ExistingStudy;
            _reconcileContext.History = _historyList == null || _historyList.Count==0? null : _historyList[0];

            LogDebugInfo(_reconcileContext);

            
            if (_historyList == null || _historyList.Count == 0)
            {
                if (ImageServerCommonConfiguration.EnablePatientsNameAutoCorrection && TryAutoCorrection(file))
                {
                    return;
                }
            
                Platform.Log(LogLevel.Debug, "Scheduling new manual reconciliation...");
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule new reconciliation"))
                {
                    MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);
                    InsertSIQReconcileStudyCommand updateStudyCommand = new InsertSIQReconcileStudyCommand(_reconcileContext);
                    
                    processor.AddCommand(updateStudyCommand);
                    processor.AddCommand(moveFileCommand);
                    processor.AddCommand(new OpValidationCommand(_reconcileContext));
                    if (processor.Execute() == false)
                    {
                        throw new ApplicationException(String.Format("Unable to schedule image reconcilation : {0}", processor.FailureReason));
                    }
                }
                Platform.Log(LogLevel.Info, "SOP {0} has been scheduled for manual reconciliation.", _sopInstanceUid);
            }
            else
            {
                Platform.Log(LogLevel.Debug, "Scheduling Auto Reconciliation...");
                
                if (_reconcileContext.History.DestStudyStorageKey != null)
                {
                    StudyStorage destStorage = StudyStorage.Load(_reconcileContext.History.DestStudyStorageKey);
                    Debug.Assert(destStorage != null);

                    _reconcileContext.DestinationStudyLocation = StudyStorageLocation.FindStorageLocations(destStorage)[0];
                    Debug.Assert(_reconcileContext.DestinationStudyLocation != null);

                    Study destStudy = Study.Find(destStorage.StudyInstanceUid, _partition); // note: assuming destination partition is the same as the current one
                    Debug.Assert(destStudy != null);
                }

                // Insert 'ReconcileStudy' in the work queue
                _reconcileContext.StoragePath = GetSuggestedTemporaryReconcileFolderPath(); // since we no longer have the record in the Study Integrity Queue
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule ReconcileStudy request based on histrory"))
                {
                    InsertReconcileStudyCommand insertCommand = new InsertReconcileStudyCommand(_reconcileContext);
                    MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);
                    
                    processor.AddCommand(insertCommand);
                    processor.AddCommand(moveFileCommand);
                    processor.AddCommand(new OpValidationCommand(_reconcileContext));
                    
                    if (processor.Execute() == false)
                    {
                        throw new ApplicationException(String.Format("Unable to create ReconcileStudy request: {0}", processor.FailureReason));
                    }

                    Debug.Assert(insertCommand.ReconcileStudyWorkQueueItem != null);
                }
                Platform.Log(LogLevel.Info, "SOP {0} has been scheduled for auto reconciliation.", _sopInstanceUid);
                
            }
        }

        private void LogDebugInfo(ReconcileImageContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Image To Be Reconciled:\n");
            sb.AppendFormat("\tSOP={0}\n", _sopInstanceUid);
            sb.AppendFormat("\tDuplicate={0}\n", context.IsDuplicate);
            sb.AppendFormat("\tExisting Patient={0}\n", ExistingStudy.PatientsName);
            sb.AppendFormat("\tExisting Study={0}\n", ExistingStudy.StudyInstanceUid);
            sb.AppendFormat("\tReferenced History record to be used: {0}\n", context.History == null ? "N/A" : context.History.Key.ToString());
            sb.AppendFormat("\tCan reconcile automatically? {0}\n", (_historyList != null && _historyList.Count > 0) ? "Yes" : "No");
            
            Platform.Log(LogLevel.Info, sb.ToString());

        }

        #endregion

    }
}
