using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class PatientBreedDescription : StringPropertyFilter<Study>
    {
        public PatientBreedDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientBreedDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
        {
            if (!IsCriterionWildcard)
                return from study in query where study.PatientBreedDescription == CriterionValue select study;

            return from study in query where SqlMethods.Like(study.PatientBreedDescription, CriterionValue) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientBreedDescription);
        }
    }
 
    internal class PatientBreedCodeSequence
    {
        internal class CodingSchemeDesignator : StringPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientBreedCodeSequenceCodingSchemeDesignator == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientBreedCodeSequenceCodingSchemeDesignator, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodingSchemeDesignator);
            }
        }

        internal class CodeValue : StringPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientBreedCodeSequenceCodeValue == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientBreedCodeSequenceCodeValue, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodeValue);
            }
        }

        internal class CodeMeaning : StringPropertyFilter<Study>
        {
            public CodeMeaning(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodeMeaning), criteria)
            {
            }

            protected override IQueryable<Study> AddToQuery(IQueryable<Study> query)
            {
                if (!IsCriterionWildcard)
                    return from study in query where study.PatientBreedCodeSequenceCodeMeaning == CriterionValue select study;

                return from study in query where SqlMethods.Like(study.PatientBreedCodeSequenceCodeMeaning, CriterionValue) select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodeMeaning);
            }
        }
    }  
}
