using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using System.Data.Linq.SqlClient;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{

    #region Strings

    internal class PatientsNameFilter : StringPropertyFilter<Study>
    {
        public PatientsNameFilter(DicomAttributeCollection inputCriteria)
            : base(DicomTags.PatientsName, inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.PatientsName == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.PatientsName, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsName);
        }
    }

    internal class ReferringPhysiciansNameFilter : StringPropertyFilter<Study>
    {
        public ReferringPhysiciansNameFilter(DicomAttributeCollection inputCriteria)
            : base(DicomTags.ReferringPhysiciansName, inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.ReferringPhysiciansName == CriterionValue select study;

            return from study in inputQuery
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
        public PatientIdFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.PatientId), inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.PatientId == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.PatientId, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientId);
        }
    }

    internal class StudyIdFilter : StringPropertyFilter<Study>
    {
        public StudyIdFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.StudyId), inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.StudyId == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.StudyId, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyId);
        }
    }

    internal class StudyDescriptionFilter : StringPropertyFilter<Study>
    {
        public StudyDescriptionFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.StudyDescription), inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.StudyDescription == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.StudyDescription, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyDescription);
        }
    }

    internal class AccessionNumberFilter : StringPropertyFilter<Study>
    {
        public AccessionNumberFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.AccessionNumber), inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.AccessionNumber == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.AccessionNumber, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.AccessionNumber);
        }
    }

    internal class PatientsSexFilter : StringPropertyFilter<Study>
    {
        public PatientsSexFilter(DicomAttributeCollection inputCriteria)
            : base(DicomTags.PatientsSex, inputCriteria)
        {
        }

        public override IQueryable<Study> AddToQuery(IQueryable<Study> inputQuery)
        {
            if (!IsCriterionWildcard)
                return from study in inputQuery where study.PatientsSex == CriterionValue select study;

            return from study in inputQuery where SqlMethods.Like(study.PatientsSex, CriterionValue) select study;
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
        public StudyInstanceUidFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.StudyInstanceUid), inputCriteria)
        {
        }

        protected override IQueryable<Study> AddUidToQuery(IQueryable<Study> inputQuery, string uid)
        {
            return from study in inputQuery where study.StudyInstanceUid == uid select study;
        }

        protected override IQueryable<Study> AddUidsToQuery(IQueryable<Study> inputQuery, string[] uids)
        {
            return from study in inputQuery where uids.Contains(study.StudyInstanceUid) select study;
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
        public StudyDateFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.StudyDate), inputCriteria)
        {
        }

        protected override IQueryable<Study> AddAfterDateToQuery(IQueryable<Study> inputQuery, System.DateTime date)
        {
            return from study in inputQuery where study.StudyDate == null || study.StudyDate >= date select study;
        }

        protected override IQueryable<Study> AddBeforeDateToQuery(IQueryable<Study> inputQuery, System.DateTime date)
        {
            return from study in inputQuery where study.StudyDate == null || study.StudyDate <= date select study;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> inputQuery,
                                                                    System.DateTime startDate, System.DateTime endDate)
        {
            return from study in inputQuery
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
        public PatientsBirthDateFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.PatientsBirthDate), inputCriteria)
        {
        }

        protected override IQueryable<Study> AddAfterDateToQuery(IQueryable<Study> inputQuery, System.DateTime date)
        {
            //return from study in inputQuery where study.PatientsBirthDate == null || study.PatientsBirthDate >= date);
            //TODO (Marmot): add column to database.
            return inputQuery;
        }

        protected override IQueryable<Study> AddBeforeDateToQuery(IQueryable<Study> inputQuery, System.DateTime date)
        {
            return inputQuery;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> inputQuery,
                                                                    System.DateTime startDate, System.DateTime endDate)
        {
            return inputQuery;
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
        public NumberOfStudyRelatedInstancesFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedInstances), inputCriteria)
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
        public NumberOfStudyRelatedSeriesFilter(DicomAttributeCollection inputCriteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedSeries), inputCriteria)
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