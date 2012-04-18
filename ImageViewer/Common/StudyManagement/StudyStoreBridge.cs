using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StudyStoreBridge : IDisposable
    {
        private IStudyStoreQuery _real;

        public StudyStoreBridge()
            :this(Platform.GetService<IStudyStoreQuery>())
        {
        }

        public StudyStoreBridge(IStudyStoreQuery real)
        {
            _real = real;
        }

        public int GetStudyCount()
        {
            return _real.GetStudyCount(new GetStudyCountRequest()).StudyCount;
        }

        public int GetStudyCount(StudyRootStudyIdentifier criteria)
        {
            return _real.GetStudyCount(new GetStudyCountRequest
                                           {
                                               Criteria = new StudyEntry{Study = criteria}
                                           }).StudyCount;
        }

        public IList<StudyEntry> GetStudyEntries()
        {
            return GetStudyEntries(null as StudyEntry);
        }

        public IList<StudyEntry> GetStudyEntries(StudyRootStudyIdentifier criteria)
        {
            return GetStudyEntries(new StudyEntry {Study = criteria});
        }

        public IList<StudyEntry> GetStudyEntries(StudyEntry criteria)
        {
            return _real.GetStudyEntries(new GetStudyEntriesRequest {Criteria = criteria}).StudyEntries;
        }

        public IList<StudyEntry> QueryByAccessionNumber(string accessionNumber)
        {
            return _real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { AccessionNumber = accessionNumber }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByPatientId(string patientId)
        {
            return _real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { PatientId = patientId }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByStudyInstanceUid(string studyInstanceUid)
        {
            return _real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { StudyInstanceUid = studyInstanceUid }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids)
        {
            return _real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           StudyInstanceUid = DicomStringHelper.GetDicomStringArray(studyInstanceUids)
                                                       }
                                       }
                    }).StudyEntries;
        }

        public IList<SeriesEntry> GetSeriesEntries(string studyInstanceUid)
        {
            return _real.GetSeriesEntries(
                new GetSeriesEntriesRequest
                {
                    Criteria = new SeriesEntry
                    {
                        Series = new SeriesIdentifier { StudyInstanceUid = studyInstanceUid }
                    }
                }).SeriesEntries;
        }

        public IList<ImageEntry> GetImageEntries(string studyInstanceUid, string seriesInstanceUid)
        {
            return _real.GetImageEntries(
                new GetImageEntriesRequest
                {
                    Criteria = new ImageEntry
                    {
                        Image = new ImageIdentifier{StudyInstanceUid = studyInstanceUid, SeriesInstanceUid = seriesInstanceUid}
                    }
                }).ImageEntries;
        }

        public void Dispose()
        {
            if (_real == null)
                return;

            var disposable = _real as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _real = null;
        }
    }
}
