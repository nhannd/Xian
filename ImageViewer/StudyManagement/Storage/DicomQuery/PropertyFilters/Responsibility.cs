using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class ResponsiblePerson : StringPropertyFilter<Study>
    {
        public ResponsiblePerson(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePerson), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.ResponsiblePerson == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.ResponsiblePerson, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePerson);
        }
    }

    internal class ResponsiblePersonRole : StringPropertyFilter<Study>
    {
        public ResponsiblePersonRole(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePersonRole), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.ResponsiblePersonRole == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.ResponsiblePersonRole, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePersonRole);
        }
    }

    internal class ResponsibleOrganization : StringPropertyFilter<Study>
    {
        public ResponsibleOrganization(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsibleOrganization), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.ResponsibleOrganization == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.ResponsibleOrganization, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsibleOrganization);
        }
    }
}