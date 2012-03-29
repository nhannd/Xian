using System;
using System.Linq;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal abstract class TimePropertyFilter<T> : PropertyFilter<T>
    {
        private bool _isRange;
        private TimeSpan? _time1;
        private TimeSpan? _time2;
        private bool _parsedCriterion;

        protected TimePropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
            Platform.CheckTrue(path.ValueRepresentation.Name == "TM", "Path is not VR=TM");
            if (Criterion != null)
                Platform.CheckTrue(Criterion.Tag.VR.Name == "TM", "Criteria is not VR=TM");
        }

        public TimeSpan? Time1
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();

                return _time1;
            }
        }

        public TimeSpan? Time2
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();

                return _time2;
            }
        }

        private void ParseCriterion()
        {
            //TODO (Marmot): We haven't implemented a time parser.
            //TimeParser.Parse(Criterion.GetString(0, ""), out _time1, out _time2, out _isRange);
        }

        protected abstract IQueryable<T> AddEqualsToQuery(IQueryable<T> query, TimeSpan time);

        protected abstract IQueryable<T> AddLessOrEqualToQuery(IQueryable<T> query, TimeSpan time);

        protected abstract IQueryable<T> AddGreaterOrEqualToQuery(IQueryable<T> query, TimeSpan time);

        protected abstract IQueryable<T> AddBetweenDatesToQuery(IQueryable<T> query, TimeSpan startTime, TimeSpan endTime);

        protected override IQueryable<T> AddToQuery(IQueryable<T> query)
        {
            if (Time1 != null && Time2 != null)
                return AddBetweenDatesToQuery(query, Time1.Value, Time2.Value);

            if (Time1 != null)
            {
                if (_isRange)
                    return AddGreaterOrEqualToQuery(query, Time1.Value);

                return AddEqualsToQuery(query, Time1.Value);
            }

            if (Time2 != null)
                return AddLessOrEqualToQuery(query, Time2.Value);

            return base.AddToQuery(query);
        }
    }
}