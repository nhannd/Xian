using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using System.Data.Linq.SqlClient;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{

    #region Strings

    internal class PatientsNameFilter : StringPropertyFilter<Study>
    {
        public PatientsNameFilter(DicomAttributeCollection criteria)
            : base(DicomTags.PatientsName, criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.PatientsName == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.PatientsName, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsName);
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

    internal class PatientIdFilter : StringPropertyFilter<Study>
    {
        public PatientIdFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientId), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.PatientId == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.PatientId, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientId);
        }
    }

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

    internal class PatientsSexFilter : StringPropertyFilter<Study>
    {
        public PatientsSexFilter(DicomAttributeCollection criteria)
            : base(DicomTags.PatientsSex, criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.PatientsSex == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.PatientsSex, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsSex);
        }
    }

    #endregion

    #region Uids

    internal class StudyInstanceUidFilter : UidPropertyFilter<Study>
    {
        public StudyInstanceUidFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyInstanceUid), criteria)
        {
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

    #endregion

    #region Dates

    //TODO (Marmot): StudyTime, ModalitiesInStudy
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

    internal class PatientsBirthDateFilter : DatePropertyFilter<Study>
    {
        public PatientsBirthDateFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientsBirthDate), criteria)
        {
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            //return from study in query where study.PatientsBirthDate == null || study.PatientsBirthDate >= date);
            //TODO (Marmot): add column to database.
            return query;
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, DateTime date)
        {
            return query;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            return query;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> query, DateTime startDate, System.DateTime endDate)
        {
            return query;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsBirthDateRaw);
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