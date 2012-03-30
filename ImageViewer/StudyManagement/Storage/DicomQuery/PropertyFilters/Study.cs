using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using System.Data.Linq.SqlClient;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class StudyInstanceUid : UidPropertyFilter<Study>
    {
        public StudyInstanceUid(DicomAttributeCollection criteria)
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

    internal class StudyId : StringPropertyFilter<Study>
    {
        public StudyId(DicomAttributeCollection criteria)
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

    internal class StudyDescription : StringPropertyFilter<Study>
    {
        public StudyDescription(DicomAttributeCollection criteria)
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

    internal class AccessionNumber : StringPropertyFilter<Study>
    {
        public AccessionNumber(DicomAttributeCollection criteria)
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

    internal class ProcedureCodeSequence
    {
        internal class CodeValue : StringPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.ProcedureCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.ProcedureCodeSequenceCodeValue == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.ProcedureCodeSequenceCodeValue, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.ProcedureCodeSequenceCodeValue);
            }
        }

        internal class CodingSchemeDesignator : StringPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.ProcedureCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.ProcedureCodeSequenceCodingSchemeDesignator == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.ProcedureCodeSequenceCodingSchemeDesignator, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.ProcedureCodeSequenceCodingSchemeDesignator);
            }
        }
    }

    //TODO (Marmot): Still need to make this work.
    internal class ModalitiesInStudy : StringPropertyFilter<Study>
    {
        public ModalitiesInStudy(DicomAttributeCollection criteria)
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

    internal class ReferringPhysiciansName : StringPropertyFilter<Study>
    {
        public ReferringPhysiciansName(DicomAttributeCollection criteria)
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

    #endregion

    #region Dates

    internal class StudyDate : DatePropertyFilter<Study>
    {
        public StudyDate(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyDate), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, DateTime date)
        {
            return from study in query where study.StudyDate == null || study.StudyDate == date select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            return from study in query where study.StudyDate == null || study.StudyDate >= date select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, DateTime date)
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

    internal class StudyTime : TimePropertyFilter<Study>
    {
        public StudyTime(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyTime), criteria)
        {
        }

        //NOTE (Marmot): None if this will get called currently because the base class doesn't work.
        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query where study.StudyTimeTicks == null || study.StudyTimeTicks == timeTicks select study;
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
                       || (study.StudyTimeTicks >= startTimeTicks && study.StudyTimeTicks <= endTimeTicks)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyTimeRaw);
        }
    }

    #endregion

    #region Non-Queryable

    internal class NumberOfStudyRelatedInstances : PropertyFilter<Study>
    {
        public NumberOfStudyRelatedInstances(DicomAttributeCollection criteria)
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

    internal class NumberOfStudyRelatedSeries : PropertyFilter<Study>
    {
        public NumberOfStudyRelatedSeries(DicomAttributeCollection criteria)
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