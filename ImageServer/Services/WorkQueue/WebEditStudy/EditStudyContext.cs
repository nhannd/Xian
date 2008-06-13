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
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// The context operation for <see cref="EditStudyCommand"/>
    /// </summary>
    /// <remarks>
    /// When the operation is committed, it assumes <see cref="Study"/>, <see cref="Patient"/> contains the updated information.
    /// Therefore, data in <see cref="Study"/> and <see cref="Patient"/>  should be updated before the <cref="UpdateContext"/> is committed.
    /// </remarks>
    public class EditStudyContext
    {
        #region Private members
        private IUpdateContext _updateContext;
        private Model.WorkQueue _workQueueItem;
        private ServerPartition _partition;
        private Study _study;
        private Patient _patient;
        private StudyStorageLocation _oldStorageLocation;
        private StudyStorageLocation _storageLocation;
        private DicomFile _currentFile;
        private string   _tempOutFolder;
        private StudyXml _newStudyXml;
        #endregion Private members

        #region Public Properties

        /// <summary>
        /// The <see cref="Model.WorkQueue"/> item being processed
        /// </summary>
        public Model.WorkQueue WorkQueueItem
        {
            get { return _workQueueItem; }
            set { _workQueueItem = value; }
        }


        /// <summary>
        /// The <see cref="DicomFile"/> being editted.
        /// </summary>
        public DicomFile CurrentFile
        {
            get { return _currentFile; }
            set { _currentFile = value; }
        }

        /// <summary>
        /// The up-to-dated information of the study being editted.
        /// </summary>
        public Study Study
        {
            get
            {
                return _study;
            }
            set
            {
                _study = value;
            }
        }

        /// <summary>
        /// The up-to-dated information of the patient of the study being editted
        /// </summary>
        public Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        /// <summary>
        /// The up-to-dated information of the location of the study being editted
        /// </summary>
        public StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
            set { _storageLocation = value; }
        }

        /// <summary>
        /// The output folder where edited study will be stored
        /// </summary>
        public string TempOutRootFolder
        {
            get 
            {
                return _tempOutFolder; 
            }
            set { _tempOutFolder = value; }
        }

        /// <summary>
        /// The <see cref="ServerPartition"/> to which the study belongs
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        /// <summary>
        /// The new <see cref="StudyXml"/> that is used to store the new study header information
        /// </summary>
        /// <remarks>
        /// The new <see cref="StudyXml"/> should not be used to obtained the new study information as it may not
        /// contain all new study information that specified in the EditStudy work queue item.
        /// </remarks>
        public StudyXml NewStudyXml
        {
            get { return _newStudyXml; }
            set { _newStudyXml = value; }
        }

        /// <summary>
        /// Gets or sets the original location of the study on the filesystem.
        /// </summary>
        public StudyStorageLocation OriginalStorageLocation
        {
            get { return _oldStorageLocation; }
            set { _oldStorageLocation = value; }
        }

        /// <summary>
        /// The <see cref="IUpdateContext"/> associated with the study edit operation.
        /// </summary>
        /// <remarks>
        /// Application/components can use this context to access the database. As all components involved
        /// in the study edit operation shared the same context, it should only not be committed and rolled back by the creator.
        /// </remarks>
        public IUpdateContext UpdateContext
        {
            get { return _updateContext; }
            set { _updateContext = value; }
        }

        #endregion

    }
}
