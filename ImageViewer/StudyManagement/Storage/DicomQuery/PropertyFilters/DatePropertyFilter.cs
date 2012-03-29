using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal abstract class DatePropertyFilter<T> : PropertyFilter<T>
    {
        private DateTime? _dateTime1;
        private DateTime? _dateTime2;
        private bool _parsedCriterion;

        protected DatePropertyFilter(DicomTagPath path, DicomAttributeCollection inputCriteria)
            : base(path, inputCriteria)
        {
        }

        public DateTime? DateTime1
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();
                
                return _dateTime1;
            }
        }

        public DateTime? DateTime2
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();

                return _dateTime2;
            }
        }

        private void ParseCriterion()
        {
            bool isRange;
            DateRangeHelper.Parse(InputCriterion.GetString(0, ""), out _dateTime1, out _dateTime2, out isRange);
        }

        protected abstract System.Linq.IQueryable<T> AddBeforeDateToQuery(IQueryable<T> inputQuery, DateTime dateTime);

        protected abstract System.Linq.IQueryable<T> AddAfterDateToQuery(IQueryable<T> inputQuery, DateTime dateTime);

        protected abstract System.Linq.IQueryable<T> AddBetweenDatesToQuery(IQueryable<T> inputQuery, DateTime startDate, DateTime endDate);

        public override System.Linq.IQueryable<T> AddToQuery(System.Linq.IQueryable<T> inputQuery)
        {
            if (DateTime1 != null && DateTime2 != null)
                return AddBetweenDatesToQuery(inputQuery, DateTime1.Value, DateTime2.Value);
            else if (DateTime1 != null)
                return AddAfterDateToQuery(inputQuery, DateTime1.Value);
            else if (DateTime2 != null)
                return AddBeforeDateToQuery(inputQuery, DateTime2.Value);

            return base.AddToQuery(inputQuery);
        }
    }
}
