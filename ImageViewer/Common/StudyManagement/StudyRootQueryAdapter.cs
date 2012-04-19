using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StudyRootQueryAdapter : IStudyStoreQuery, IDisposable
    {
        private IStudyRootQuery _studyRootQuery;

        public StudyRootQueryAdapter(IStudyRootQuery studyRootQuery)
        {
            Platform.CheckForNullReference(studyRootQuery, "studyRootQuery");
            _studyRootQuery = studyRootQuery;
        }

        #region IStudyStoreQuery Members

        GetStudyCountResult IStudyStoreQuery.GetStudyCount(GetStudyCountRequest request)
        {
            throw new NotSupportedException("IStudyRootQuery does not support study count queries.");
        }

        public GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new StudyEntry();

            if (request.Criteria.Study == null)
                request.Criteria.Study = new StudyRootStudyIdentifier();

            return new GetStudyEntriesResult
                       {
                           StudyEntries = _studyRootQuery.StudyQuery(request.Criteria.Study)
                            .Select(s => new StudyEntry {Study = s}).ToList()
                       };
        }

        public GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new SeriesEntry();

            if (request.Criteria.Series == null)
                request.Criteria.Series = new SeriesIdentifier();

            return new GetSeriesEntriesResult
            {
                SeriesEntries = _studyRootQuery.SeriesQuery(request.Criteria.Series)
                 .Select(s => new SeriesEntry { Series = s }).ToList()
            };
        }

        public GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new ImageEntry();

            if (request.Criteria.Image == null)
                request.Criteria.Image = new ImageIdentifier();

            return new GetImageEntriesResult
            {
                ImageEntries = _studyRootQuery.ImageQuery(request.Criteria.Image)
                 .Select(i => new ImageEntry { Image = i }).ToList()
            };
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_studyRootQuery == null)
                return;

            var disposable = _studyRootQuery as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _studyRootQuery = null;
        }

        #endregion
    }
}
