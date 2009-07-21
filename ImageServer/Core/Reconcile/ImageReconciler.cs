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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    /// <summary>
    /// A helper class to resolve the location of the images that needs to be reconciled.
    /// Used by <see cref="ImageReconciler"/>.
    /// </summary>
    class ReconcileStorage
    {
        #region Private Members
        readonly private string _folder;
        private readonly StudyStorageLocation _studyLocation;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ReconcileStorage"/>
        /// </summary>
        /// <param name="studyLocation">The <see cref="StudyStorageLocation"/> of the study which contains the images to be reconciled.</param>
        /// <param name="folder">The name of the folder used for storing the images to be reconciled.</param>
        public ReconcileStorage(StudyStorageLocation studyLocation, string folder)
        {
            Platform.CheckForNullReference(studyLocation, "studyLocation");
            Platform.CheckForEmptyString(folder, "folder");

            _studyLocation = studyLocation;
            _folder = folder;
        }
        
        #endregion

        /// <summary>
        /// Gets the path to the folder that contains the images to be reconciled.
        /// </summary>
        /// <returns></returns>
        public string GetFolderPath()
        {
            string path = Path.Combine(_studyLocation.FilesystemPath, _studyLocation.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, _folder);
            return Path.Combine(path, _studyLocation.StudyInstanceUid);
        }
        
        /// <summary>
        /// Gets the path to the images, relative to the path returned by <see cref="GetFolderPath"/>
        /// </summary>
        /// <param name="seriesUid"></param>
        /// <param name="sopUid"></param>
        /// <returns></returns>
        public string GetSopRelativePath(string seriesUid, string sopUid)
        {
            return sopUid  + ".dcm";
        }

        
        /// <summary>
        /// Gets the path to the image that needs to be reconciled.
        /// </summary>
        /// <param name="seriesUid"></param>
        /// <param name="sopUid"></param>
        /// <returns></returns>
        public string GetSopInstancePath(string seriesUid, string sopUid)
        {
            string path = Path.Combine(GetFolderPath(), GetSopRelativePath(seriesUid, sopUid));
            return path;
        }

    }

    /// <summary>
    /// Helper class to insert a <see cref="StudyIntegrityQueue"/> for manual image reconciliation.
    /// </summary>
	class ImageReconciler
	{
		#region Private Members
        private readonly SopProcessingContext _context;
	    #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ImageReconciler"/>
        /// </summary>
        /// <param name="context"></param>
        public ImageReconciler(SopProcessingContext context)
        {
            _context = context;
        }
        
        #endregion


		#region Public Methods
        /// <summary>
        /// Inserts a <see cref="StudyIntegrityQueue"/> entry for manual reconciliation.
        /// </summary>
        /// <param name="file">The DICOM file that needs to be reconciled.</param>
        /// <param name="reason">The type of <see cref="StudyIntegrityQueue"/> entry to be inserted.</param>
        /// <remarks>
        /// A copy of the DICOM file will be stored in a special folder allocated for 
        /// reconciliation purpose. The caller is responsible for managing the original copy.
        /// </remarks>
		public void ScheduleReconcile(DicomFile file, StudyIntegrityReasonEnum reason)
		{
            Platform.CheckForNullReference(_context.StudyLocation, "_context.StudyLocation");
          
            Platform.Log(LogLevel.Info, "Scheduling new manual reconciliation for SOP {0}", file.MediaStorageSopInstanceUid);
            ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(_context.StudyLocation.FilesystemKey);
            Platform.CheckForNullReference(fs, "fs");
            
            ReconcileStorage reconcileStorage = new ReconcileStorage(_context.StudyLocation, _context.UidGroup); 

            using(ServerCommandProcessor processor = new ServerCommandProcessor("Schedule Manual Reconciliation"))
            {
                string path = reconcileStorage.GetSopInstancePath(file.DataSet[DicomTags.SeriesInstanceUid].ToString(), file.DataSet[DicomTags.SopInstanceUid].ToString());
                DirectoryInfo dir = new DirectoryInfo(path);
                CreateDirectoryCommand mkdir = new CreateDirectoryCommand(dir.Parent.FullName);
                processor.AddCommand(mkdir);

                SaveDicomFileCommand saveFileCommand = new SaveDicomFileCommand(path, file, true, true);
                processor.AddCommand(saveFileCommand);

                InsertSIQCommand updateStudyCommand = new InsertSIQCommand(_context.StudyLocation, reason, file, _context.UidGroup, reconcileStorage);
                processor.AddCommand(updateStudyCommand);

                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to schedule image reconcilation : {0}", processor.FailureReason), processor.FailureException);
                }
            }


		}

		#endregion
	}

    /// <summary>
    /// Command to insert a <see cref="StudyIntegrityQueue"/> entry.
    /// </summary>
    internal class InsertSIQCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly StudyStorageLocation _storageLocation;
        private readonly StudyIntegrityReasonEnum _reason;
        private readonly DicomFile _file;
        private readonly ReconcileStorage _reconcileImageStorage;

        private StudyIntegrityQueue _siqItem;
        private readonly string _uidGroup; 
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="InsertSIQCommand"/> to insert a <see cref="StudyIntegrityQueue"/> entry
        /// for a <see cref="DicomFile"/>
        /// </summary>
        /// <param name="studyStorage">The <see cref="StudyStorageLocation"/> of the study</param>
        /// <param name="reason">A <see cref="StudyIntegrityReasonEnum"/> value indicating the type of <see cref="StudyIntegrityQueue"/> entry to be inserted.</param>
        /// <param name="file">The DICOM file that needs to be reconciled</param>
        /// <param name="uidGroup">The string value representing the group of images which the DICOM <paramref name="file"/> belongs to</param>
        /// <param name="reconcileImageStorage">The <see cref="ReconcileStorage"/> where the DICOM file will be stored.</param>
        public InsertSIQCommand(
            StudyStorageLocation studyStorage, 
            StudyIntegrityReasonEnum reason, 
            DicomFile file,
            string uidGroup,
            ReconcileStorage reconcileImageStorage
            )
            :base("Insert SIQ Command", true)
        {
            _storageLocation = studyStorage;
            _reconcileImageStorage = reconcileImageStorage;
            _reason = reason;
            _file = file;
            _uidGroup = uidGroup;
        }

        /// <summary>
        /// Gets the <see cref="StudyIntegrityQueue"/> inserted by this command.
        /// </summary>
        public StudyIntegrityQueue SIQEntry
        {
            get { return _siqItem; }
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            string seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            string sopUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            ImageSetDescriptor imageSet = new ImageSetDescriptor(_file.DataSet);
            ReconcileStudyQueueDescription queueDesc = GetQueueEntryDescription(_storageLocation, _file);
            
            IInsertStudyIntegrityQueue broker = updateContext.GetBroker<IInsertStudyIntegrityQueue>();
            InsertStudyIntegrityQueueParameters parameters = new InsertStudyIntegrityQueueParameters();
            parameters.Description = queueDesc.ToString();
            parameters.StudyInstanceUid = _storageLocation.StudyInstanceUid;
            parameters.ServerPartitionKey = _storageLocation.ServerPartition.Key;
            parameters.StudyStorageKey = _storageLocation.Key;
            parameters.StudyIntegrityReasonEnum = _reason;
            parameters.SeriesInstanceUid = seriesUid;
            parameters.SeriesDescription = sopUid;
            parameters.SopInstanceUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            parameters.StudyData = XmlUtils.SerializeAsXmlDoc(imageSet); // this is used by the stored proc on initial insert only 
            parameters.GroupID = _uidGroup;
            parameters.UidRelativePath = _reconcileImageStorage.GetSopRelativePath(_file.DataSet[DicomTags.SeriesInstanceUid].ToString(), _file.DataSet[DicomTags.SopInstanceUid].ToString()); 
            StudyIntegrityQueue item = broker.FindOne(parameters);
            if (item == null)
            {
                throw new ApplicationException("Unable to update reconcile queue");
            }

            _siqItem = item;
            if (item.QueueData == null)
            {
                ReconcileStudyWorkQueueData data = new ReconcileStudyWorkQueueData();
                data.StoragePath = _reconcileImageStorage.GetFolderPath();
                data.Details = new ImageSetDetails(_file.DataSet);
                data.Details.InsertFile(_file);
                XmlDocument xmlQueueData = XmlUtils.SerializeAsXmlDoc(data);

                item.QueueData = xmlQueueData;

                IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                updateBroker.Update(item);
            }
            else
            {
                // Need to re-use the path that's already assigned for this entry
                ReconcileStudyWorkQueueData data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.QueueData);
                data.Details.InsertFile(_file);

                XmlDocument updatedQueueDataXml = XmlUtils.SerializeAsXmlDoc(data);
                IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueUpdateColumns columns = new StudyIntegrityQueueUpdateColumns();
                columns.QueueData = updatedQueueDataXml;
                updateBroker.Update(item.GetKey(), columns);

            }
        }

        private static ReconcileStudyQueueDescription GetQueueEntryDescription(StudyStorageLocation existingStorage, DicomFile file)
        {
            ReconcileStudyQueueDescription desc = new ReconcileStudyQueueDescription();
            desc.ExistingPatientId = existingStorage.Study.PatientId;
            desc.ExistingPatientName = existingStorage.Study.PatientsName;
            desc.ExistingAccessionNumber = existingStorage.Study.AccessionNumber;

            desc.ConflictingPatientName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            desc.ConflictingPatientId = file.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
            desc.ConflictingAccessionNumber = file.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);

            return desc;
        }
    }
}