using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    internal class PatientSpeciesDescription : StringDicomPropertyFilter<Study>
    {
        public PatientSpeciesDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientSpeciesDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.PatientSpeciesDescription == null
                        || study.PatientSpeciesDescription == ""
                        || study.PatientSpeciesDescription == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.PatientSpeciesDescription == null
                        || study.PatientSpeciesDescription == ""
                       || SqlMethods.Like(study.PatientSpeciesDescription, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientSpeciesDescription);
        }
    }

    internal class PatientSpeciesCodeSequence
    {
        internal class CodingSchemeDesignator : StringDicomPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodingSchemeDesignator == null
                            || study.PatientSpeciesCodeSequenceCodingSchemeDesignator == ""
                            || study.PatientSpeciesCodeSequenceCodingSchemeDesignator == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodingSchemeDesignator == null
                            || study.PatientSpeciesCodeSequenceCodingSchemeDesignator == ""
                           || SqlMethods.Like(study.PatientSpeciesCodeSequenceCodingSchemeDesignator, criterion)
                       select study;
            }


            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodingSchemeDesignator);
            }
        }

        internal class CodeValue : StringDicomPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeValue == null
                            || study.PatientSpeciesCodeSequenceCodeValue == ""
                            || study.PatientSpeciesCodeSequenceCodeValue == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeValue == null
                            || study.PatientSpeciesCodeSequenceCodeValue == ""
                           || SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeValue, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeValue);
            }
        }

        internal class CodeMeaning : StringDicomPropertyFilter<Study>
        {
            public CodeMeaning(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeMeaning), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeMeaning == null
                            || study.PatientSpeciesCodeSequenceCodeMeaning == ""
                            || study.PatientSpeciesCodeSequenceCodeMeaning == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeMeaning == null
                            || study.PatientSpeciesCodeSequenceCodeMeaning == ""
                           || SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeMeaning, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeMeaning);
            }
        }
    }
}