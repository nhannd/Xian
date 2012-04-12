using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    //TODO (Marmot): Proxy and throw faults.

    public class StudyStoreQuery : IStudyStore
    {
        private readonly DicomStoreDataContext _context;

        internal StudyStoreQuery(DicomStoreDataContext context)
        {
            _context = context;
        }

        public IEnumerable<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria)
        {
            Platform.CheckForNullReference(queryCriteria, "queryCriteria");

            string level = queryCriteria[DicomTags.QueryRetrieveLevel].ToString();
            switch (level)
            {
                case "STUDY":
                    return StudyQuery(queryCriteria);
                case "SERIES":
                    return SeriesQuery(queryCriteria);
                case "IMAGE":
                    return ImageQuery(queryCriteria);
                default:
                    throw new ArgumentException(String.Format("Invalid query level: {0}", level));
            }
        }

        public GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            int count = request.QueryIdentifier == null ? _context.Studies.Count() : StudyQuery(request.QueryIdentifier).Count;
            return new GetStudyCountResult{StudyCount = count};
        }

        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            var criteria = queryCriteria.ToDicomAttributeCollection();
            var results = Query(criteria);
            return results.Select(collection => new StudyRootStudyIdentifier(collection)).ToList();
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            var criteria = queryCriteria.ToDicomAttributeCollection();
            var results = Query(criteria);
            return results.Select(collection => new SeriesIdentifier(collection)).ToList();
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            var criteria = queryCriteria.ToDicomAttributeCollection();
            var results = Query(criteria);
            return results.Select(collection => new ImageIdentifier(collection)).ToList();
        }

        private List<DicomAttributeCollection> StudyQuery(DicomAttributeCollection queryCriteria)
        {
            try
            {
                var filters = new StudyPropertyFilters(queryCriteria);
                var results = filters.Query(_context.Studies);
                return filters.ConvertResults(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the study root query.", e);
            }
        }

        private List<DicomAttributeCollection> SeriesQuery(DicomAttributeCollection queryCriteria)
        {
            string studyUid = queryCriteria[DicomTags.StudyInstanceUid];
            if (String.IsNullOrEmpty(studyUid))
                throw new ArgumentException("The study uid must be specified for a series level query.");

            IStudy study = (from s in _context.Studies where s.StudyInstanceUid == studyUid select s).FirstOrDefault();
            if (study == null)
                throw new ArgumentException(String.Format("No study exists with the given study uid ({0}).", studyUid));

            try
            {
                var filters = new PropertyFilters<Series>(queryCriteria);
                var results = filters.FilterResults(study.GetSeries().Cast<Series>());
                return filters.ConvertResults(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the series query.", e);
            }
        }

        private List<DicomAttributeCollection> ImageQuery(DicomAttributeCollection queryCriteria)
        {
            string studyUid = queryCriteria[DicomTags.StudyInstanceUid];
            string seriesUid = queryCriteria[DicomTags.SeriesInstanceUid];

            if (String.IsNullOrEmpty(studyUid) || String.IsNullOrEmpty(seriesUid))
                throw new ArgumentException("The study and series uids must be specified for an image level query.");

            IStudy study = (from s in _context.Studies where s.StudyInstanceUid == studyUid select s).FirstOrDefault();
            if (study == null)
                throw new ArgumentException(String.Format("No study exists with the given study uid ({0}).", studyUid));

            ISeries series = (from s in study.GetSeries() where s.SeriesInstanceUid == seriesUid select s).FirstOrDefault();
            if (series == null)
            {
                string message = String.Format("No series exists with the given study and series uids ({0}, {1})", studyUid, seriesUid);
                throw new ArgumentException(message);
            }

            try
            {
                var filters = new PropertyFilters<SopInstance>(queryCriteria);
                var results = filters.FilterResults(study.GetSopInstances().Cast<SopInstance>());
                return filters.ConvertResults(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the image query.", e);
            }
        }
   }
}