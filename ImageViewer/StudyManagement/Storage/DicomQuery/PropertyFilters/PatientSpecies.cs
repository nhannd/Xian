using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class PatientSpeciesDescription : StringPropertyFilter<Study>
    {
        public PatientSpeciesDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientSpeciesDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.PatientSpeciesDescription == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.PatientSpeciesDescription, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientSpeciesDescription);
        }
    }

    internal class PatientSpeciesCodeSequence
    {
        internal class CodingSchemeDesignator : StringPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientSpeciesCodeSequenceCodingSchemeDesignator == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodingSchemeDesignator, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodingSchemeDesignator);
            }
        }

        internal class CodeValue : StringPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientSpeciesCodeSequenceCodeValue == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeValue, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeValue);
            }
        }

        internal class CodeMeaning : StringPropertyFilter<Study>
        {
            public CodeMeaning(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeMeaning), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientSpeciesCodeSequenceCodeMeaning == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeMeaning, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeMeaning);
            }
        }
    }
}