using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using System.Data.Linq.SqlClient;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class StudyInstanceUidFilter : UidPropertyFilter<Study>
    {
        public StudyInstanceUidFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyInstanceUid), criteria)
        {
            IsRequired = true;
        }

        protected override IQueryable<Study> AddUidToQuery(IQueryable<Study> query, string uid)
        {
            return from study in query where study.StudyInstanceUid == uid select study;
        }

        protected override IQueryable<Study> AddUidsToQuery(IQueryable<Study> query, string[] uids)
        {
            return from study in query where uids.Contains(study.StudyInstanceUid) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyInstanceUid);
        }
    }

    #region Strings

    internal class StudyIdFilter : StringPropertyFilter<Study>
    {
        public StudyIdFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyId), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.StudyId == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.StudyId, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyId);
        }
    }

    internal class StudyDescriptionFilter : StringPropertyFilter<Study>
    {
        public StudyDescriptionFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.StudyDescription == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.StudyDescription, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyDescription);
        }
    }

    internal class AccessionNumberFilter : StringPropertyFilter<Study>
    {
        public AccessionNumberFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.AccessionNumber), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.AccessionNumber == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.AccessionNumber, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.AccessionNumber);
        }
    }

    //TODO (Marmot): Still need to make this work.
    internal class ModalitiesInStudyFilter : StringPropertyFilter<Study>
    {
        public ModalitiesInStudyFilter(DicomAttributeCollection criteria)
            : base(DicomTags.ModalitiesInStudy, criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            //TODO (Marmot): need to fill this in.
            return query;
        }

        protected override System.Collections.Generic.IEnumerable<Study> FilterResults(System.Collections.Generic.IEnumerable<Study> results)
        {
            //TODO (Marmot): need to fill this in.
            return base.FilterResults(results);
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ModalitiesInStudy);
        }
    }

    internal class ReferringPhysiciansNameFilter : StringPropertyFilter<Study>
    {
        public ReferringPhysiciansNameFilter(DicomAttributeCollection criteria)
            : base(DicomTags.ReferringPhysiciansName, criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.ReferringPhysiciansName == CriterionValue select study;

            return from study in query
                   where SqlMethods.Like(study.ReferringPhysiciansName, CriterionValue)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ReferringPhysiciansName);
        }
    }

    //ProcedureCodeSequenceCodingSchemeDesignator, ProcedureCodeSequenceCodeValue,
    #endregion

    #region Dates

    internal class StudyDateFilter : DatePropertyFilter<Study>
    {
        public StudyDateFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyDate), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, System.DateTime date)
        {
            return from study in query where study.StudyDate == null || study.StudyDate >= date select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, System.DateTime date)
        {
            return from study in query where study.StudyDate == null || study.StudyDate >= date select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, System.DateTime date)
        {
            return from study in query where study.StudyDate == null || study.StudyDate <= date select study;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> query, DateTime startDate, DateTime endDate)
        {
            return from study in query
                   where
                       study.StudyDate == null
                       || (study.StudyDate >= startDate && study.StudyDate <= endDate)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyDateRaw);
        }
    }

    #endregion

    #region Times

    internal class StudyTimeFilter : TimePropertyFilter<Study>
    {
        public StudyTimeFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyTime), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query where study.StudyTimeTicks == null || study.StudyTimeTicks >= timeTicks select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query where study.StudyTimeTicks == null || study.StudyTimeTicks >= timeTicks select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query where study.StudyTimeTicks == null || study.StudyTimeTicks <= timeTicks select study;
        }

        protected override IQueryable<Study> AddBetweenTimesToQuery(IQueryable<Study> query, long startTimeTicks, long endTimeTicks)
        {
            return from study in query
                   where
                       study.StudyTimeTicks == null
                       || (study.StudyTimeTicks >= startTimeTicks && study.StudyTimeTicks <= startTimeTicks)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyTimeRaw);
        }
    }

    #endregion

    #region Non-Queryable

    internal class NumberOfStudyRelatedInstancesFilter : PropertyFilter<Study>
    {
        public NumberOfStudyRelatedInstancesFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedInstances), criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            if (item.NumberOfStudyRelatedInstances.HasValue)
                resultAttribute.SetInt32(0, item.NumberOfStudyRelatedInstances.Value);
            else
                resultAttribute.SetNullValue();
        }
    }

    internal class NumberOfStudyRelatedSeriesFilter : PropertyFilter<Study>
    {
        public NumberOfStudyRelatedSeriesFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedSeries), criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            if (item.NumberOfStudyRelatedSeries.HasValue)
                resultAttribute.SetInt32(0, item.NumberOfStudyRelatedSeries.Value);
            else
                resultAttribute.SetNullValue();
        }
    }

    #endregion
}