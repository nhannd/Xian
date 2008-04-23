using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    public class EditStudyContext
    {
        private IUpdateContext _updateContext;

        private Model.WorkQueue _workQueueItem;
        private string _newStudyInstanceUid;
        private ServerPartition _partition;
        private ServerEntityKey _studyKey;
        private ServerEntityKey _patientKey;
        private StudyStorageLocation _storageLocation;
        private string _newStudyDate;
        
        private DicomFile _currentFile;
        private string   _destFolder;
        private StudyXml _newStudyXml;
        
        public IUpdateContext UpdateContext
        {
            get { return _updateContext; }
            set { _updateContext = value; }
        }


        public DicomFile CurrentFile
        {
            get { return _currentFile; }
            set { _currentFile = value; }
        }


        public ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }

        public ServerEntityKey PatientKey
        {
            get { return _patientKey; }
            set { _patientKey = value; }
        }

        public string DestinationFolder
        {
            get 
            {
                return _destFolder; 
            }
            set { _destFolder = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Model.WorkQueue WorkQueueItem
        {
            get { return _workQueueItem; }
            set { _workQueueItem = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
            set { _storageLocation = value; }
        }

        public string NewStudyDate
        {
            get { return _newStudyDate; }
            set { _newStudyDate = value; }
        }

        public string NewStudyInstanceUid
        {
            get { return _newStudyInstanceUid; }
            set { _newStudyInstanceUid = value; }
        }

        public StudyXml NewStudyXml
        {
            get { return _newStudyXml; }
            set { _newStudyXml = value; }
        }
    }
}
