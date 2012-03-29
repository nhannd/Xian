using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal abstract class UidPropertyFilter<T> : PropertyFilter<T>
    {
        protected UidPropertyFilter(DicomTagPath path, DicomAttributeCollection inputCriteria)
            : base(path, inputCriteria)
        {
        }

        protected string[] Uids
        {
            get
            {
                if (InputCriterion == null)
                    return null;

                //TODO (Marmot): store it?
                return DicomStringHelper.GetStringArray(InputCriterion.ToString());
            }    
        }

        protected abstract IQueryable<T> AddUidToQuery(IQueryable<T> inputQuery, string uid);
        protected abstract IQueryable<T> AddUidsToQuery(IQueryable<T> inputQuery, string[] uids);

        public override System.Linq.IQueryable<T> AddToQuery(System.Linq.IQueryable<T> inputQuery)
        {
            var uids = Uids;
            if (uids == null || uids.Length == 0)
                return base.AddToQuery(inputQuery);

            if (uids.Length == 1)
                return AddUidToQuery(inputQuery, uids[0]);

            return AddUidsToQuery(inputQuery, uids);
        }
    }
}
