using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class StudyPropertyFilters : PropertyFilters<Study>
    {
        public StudyPropertyFilters(DicomAttributeCollection criteria) : base(criteria)
        {
        }

        protected override System.Collections.Generic.List<IPropertyFilter<Study>> CreateFilters(DicomAttributeCollection criteria)
        {
            var filters = base.CreateFilters(criteria);
            var modalitiesInStudyPath = new DicomTagPath(DicomTags.ModalitiesInStudy);
            var modalitiesInStudyIndex = filters.FindIndex(f => f.Path.Equals(modalitiesInStudyPath));
            var modalitiesInStudyFilter = filters[modalitiesInStudyIndex];
            
            //Because of the potentially complex joins of the same initial query over and over, move this one to the front.
            filters.RemoveAt(modalitiesInStudyIndex);
            filters.Insert(0, modalitiesInStudyFilter);
            return filters;
        }

        protected override IQueryable<Study> Query(IQueryable<Study> initialQuery)
        {
            var query = base.Query(initialQuery);
            
            //We don't want to return anything that is scheduled to be deleted.
            query = query.Where(s => !s.Deleted);
            
            return query;
        }
    }
}
