using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class DeletedStudyDataSource 
    {
        private string _accessionNumber;
        private string _patientId;
        private string _patientsName;
        private DateTime? _studyDate;
        private string _studyDescription;
        private IList<DeletedStudyInfo> _studies;

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber  = value; }
        }

        public DeletedStudyInfo  Find(object key)
        {
            return CollectionUtils.SelectFirst<DeletedStudyInfo>(_studies,
                delegate(DeletedStudyInfo info)
                    {
                        return info.RowKey.Equals(key);
                    });
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public DateTime? StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public IEnumerable Select(int startRowIndex, int maxRows)
        {
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyDeleteRecordEntityBroker broker = ctx.GetBroker<IStudyDeleteRecordEntityBroker>();
                StudyDeleteRecordSelectCriteria criteria = new StudyDeleteRecordSelectCriteria();
                if (!String.IsNullOrEmpty(AccessionNumber))
                    criteria.AccessionNumber.Like("%"+AccessionNumber+"%");
                if (!String.IsNullOrEmpty(PatientId))
                    criteria.PatientId.Like("%" + PatientId + "%");
                if (!String.IsNullOrEmpty(PatientsName))
                    criteria.PatientsName.Like("%" + PatientsName + "%");
                if (!String.IsNullOrEmpty(StudyDescription))
                    criteria.StudyDescription.Like("%" + StudyDescription + "%");
                if (StudyDate!=null)
                    criteria.StudyDate.Like("%" + DateParser.ToDicomString(StudyDate.Value) + "%");

                criteria.Timestamp.SortDesc(0);
                IList<StudyDeleteRecord> list = broker.Find(criteria, startRowIndex, maxRows);

                _studies = CollectionUtils.Map<StudyDeleteRecord, DeletedStudyInfo>(
                    list, delegate(StudyDeleteRecord record)
                              {
                                  return DeletedStudyInfoAssembler.CreateDeletedStudyInfo(record);
                              });

                return _studies;
            }
        }

        public int SelectCount()
        {
            StudyDeleteRecordSelectCriteria criteria = new StudyDeleteRecordSelectCriteria();
            if (!String.IsNullOrEmpty(AccessionNumber))
                criteria.AccessionNumber.Like("%" + AccessionNumber + "%");
            if (!String.IsNullOrEmpty(PatientId))
                criteria.PatientId.Like("%" + PatientId + "%");
            if (!String.IsNullOrEmpty(PatientsName))
                criteria.PatientsName.Like("%" + PatientsName + "%");
            if (!String.IsNullOrEmpty(StudyDescription))
                criteria.StudyDescription.Like("%" + StudyDescription + "%");
            if (StudyDate != null)
                criteria.StudyDate.Like("%" + DateParser.ToDicomString(StudyDate.Value) + "%");
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyDeleteRecordEntityBroker broker = ctx.GetBroker<IStudyDeleteRecordEntityBroker>();
                return broker.Count(criteria);
            }
        }
    }

    internal static class DeletedStudyInfoAssembler
    {
        static private FilesystemMonitor _fsMonitor = FilesystemMonitor.Instance;

        public static DeletedStudyInfo CreateDeletedStudyInfo(StudyDeleteRecord record)
        {
            DeletedStudyInfo info = new DeletedStudyInfo();
            info.DeleteStudyRecord = record.GetKey();
            info.RowKey = record.GetKey().Key;
            info.StudyInstanceUid = record.StudyInstanceUid;
            info.PatientsName = record.PatientsName;
            info.AccessionNumber = record.AccessionNumber;
            info.PatientId = record.PatientId;
            info.StudyDate = record.StudyDate;
            info.PartitionAE = record.ServerPartitionAE;
            info.StudyDescription = record.StudyDescription;
            info.BackupFolderPath = _fsMonitor.GetFilesystemInfo(record.FilesystemKey).ResolveAbsolutePath(record.BackupPath);
            info.ReasonForDeletion = record.Reason;
            info.DeleteTime = record.Timestamp;
            if (record.ArchiveInfo!=null)
                info.Archives = XmlUtils.Deserialize<DeletedStudyArchiveInfoCollection>(record.ArchiveInfo);

            
            return info;
        }
    }
}
