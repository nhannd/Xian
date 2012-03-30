using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
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

    internal class PatientsBirthTimeFilter : TimePropertyFilter<Study>
    {
        public PatientsBirthTimeFilter(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientsBirthTime), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query
                   where study.PatientsBirthTimeTicks == null || study.PatientsBirthTimeTicks >= timeTicks
                   select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query
                   where study.PatientsBirthTimeTicks == null || study.PatientsBirthTimeTicks >= timeTicks
                   select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            return from study in query where study.PatientsBirthTimeTicks == null || study.PatientsBirthTimeTicks <= timeTicks select study;
        }

        protected override IQueryable<Study> AddBetweenTimesToQuery(IQueryable<Study> query, long startTimeTicks, long endTimeTicks)
        {
            return from study in query
                   where
                       study.PatientsBirthTimeTicks == null
                       || (study.PatientsBirthTimeTicks >= startTimeTicks && study.PatientsBirthTimeTicks <= startTimeTicks)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsBirthTimeRaw);
        }
    }
}