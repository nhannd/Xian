using System.Linq;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class StudyPropertyFilters : PropertyFilters<Study>
    {
        public StudyPropertyFilters(DicomAttributeCollection criteria) : base(criteria)
        {
        }

        protected override IQueryable<Study> Query(IQueryable<Study> initialQuery)
        {
            var query = base.Query(initialQuery);
            
            //We don't want to return anything that is about to be deleted.
            query = query.Where(s => !s.Deleted);
            
            //TODO (Marmot): Modalities in study.

            return query;
        }
    }
}
