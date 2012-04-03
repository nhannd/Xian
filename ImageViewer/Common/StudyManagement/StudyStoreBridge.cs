using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StudyStoreBridge : IStudyRootQueryBridge
    {
        private IStudyStore _real;
        private IStudyRootQueryBridge _realBridge;

        public StudyStoreBridge()
            :this(Platform.GetService<IStudyStore>())
        {
        }

        public StudyStoreBridge(IStudyStore real)
        {
            _real = real;
            _realBridge = new StudyRootQueryBridge(_real);
        }

        public IComparer<StudyRootStudyIdentifier> StudyComparer
        {
            get { return _realBridge.StudyComparer; }
            set { _realBridge.StudyComparer = value; }
        }

        public IComparer<SeriesIdentifier> SeriesComparer
        {
            get { return _realBridge.SeriesComparer; }
            set { _realBridge.SeriesComparer = value; }
        }

        public IComparer<ImageIdentifier> ImageComparer
        {
            get { return _realBridge.ImageComparer; }
            set { _realBridge.ImageComparer = value; }
        }

        public int GetStudyCount()
        {
            return _real.GetStudyCount(new GetStudyCountRequest()).StudyCount;
        }

        public int GetStudyCount(StudyRootStudyIdentifier queryIdentifier)
        {
            return _real.GetStudyCount(new GetStudyCountRequest{QueryIdentifier = queryIdentifier}).StudyCount;
        }

        public IList<StudyRootStudyIdentifier> QueryByAccessionNumber(string accessionNumber)
        {
            return _realBridge.QueryByAccessionNumber(accessionNumber);
        }

        public IList<StudyRootStudyIdentifier> QueryByPatientId(string patientId)
        {
            return _realBridge.QueryByPatientId(patientId);
        }

        public IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(string studyInstanceUid)
        {
            return _realBridge.QueryByStudyInstanceUid(studyInstanceUid);
        }

        public IList<StudyRootStudyIdentifier> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids)
        {
            return _realBridge.QueryByStudyInstanceUid(studyInstanceUids);
        }

        public IList<SeriesIdentifier> SeriesQuery(string studyInstanceUid)
        {
            return _realBridge.SeriesQuery(studyInstanceUid);
        }

        public IList<ImageIdentifier> ImageQuery(string studyInstanceUid, string seriesInstanceUid)
        {
            return _realBridge.ImageQuery(studyInstanceUid, seriesInstanceUid);
        }

        public IList<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria)
        {
            return _realBridge.Query(queryCriteria);
        }

        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            return _realBridge.StudyQuery(queryCriteria);
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            return _realBridge.SeriesQuery(queryCriteria);
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            return _realBridge.ImageQuery(queryCriteria);
        }

        public void Dispose()
        {
            if (_realBridge == null)
                return;

            _realBridge.Dispose();
            _realBridge = null;

            var disposable = _real as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _real = null;
        }
    }
}
