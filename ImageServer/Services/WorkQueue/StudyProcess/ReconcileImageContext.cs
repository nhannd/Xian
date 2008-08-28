using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class ReconcileImageContext
    {
        private readonly DicomMessageBase _message;
        private readonly StudyStorageLocation _studyLocation;
        private ReconcileQueue _reconcileRecord;
        private ServerFilesystemInfo _filesystemInfo;
        private string _srcImagePath;
        private string _destImagePath;
        private ServerPartition _partition;

        public ReconcileImageContext(ServerPartition partition, StudyStorageLocation studyLocation, DicomMessageBase message, string imagePath)
        {
            _partition = partition;
            _studyLocation = studyLocation;

            _message = message;
            _srcImagePath = imagePath;
            _filesystemInfo = FilesystemMonitor.Instance.GetFilesystemInfo(studyLocation.FilesystemKey);

        }

        public DicomMessageBase Message
        {
            get { return _message; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
        }

        public string AccessionNumber
        {
            get
            {
                return _message.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);
            }
        }

        public string IssuerOfPatientId
        {
            get
            {
                return _message.DataSet[DicomTags.IssuerOfPatientId].GetString(0, String.Empty);
            }
        }
        public string PatientId
        {
            get
            {
                return _message.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
            }
        }
        public string PatientsBirthDate
        {
            get
            {
                return _message.DataSet[DicomTags.PatientsBirthDate].GetString(0, String.Empty);
            }
        }

        public string PatientsName
        {
            get
            {
                return _message.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            }
        }

        public string PatientsSex
        {
            get
            {
                return _message.DataSet[DicomTags.PatientsSex].GetString(0, String.Empty);
            }
        }

        public string StudyInstanceUid
        {
            get
            {
                return _message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            }
        }

        public string SeriesInstanceUid
        {
            get
            {
                return _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            }
        }

        public string SopInstanceUid
        {
            get
            {
                return _message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            }
        }


        public string SourceImagePath
        {
            get { return _srcImagePath; }
            set { _srcImagePath = value; }
        }

        public ReconcileQueue ReconcileRecord
        {
            get { return _reconcileRecord; }
            set { _reconcileRecord = value; }
        }


        public ServerFilesystemInfo FilesystemInfo
        {
            get { return _filesystemInfo; }
            set { _filesystemInfo = value; }
        }

        public string SrcImagePath
        {
            get { return _srcImagePath; }
            set { _srcImagePath = value; }
        }

        public string DestImagePath
        {
            get { return _destImagePath; }
            set { _destImagePath = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }
    }

}
