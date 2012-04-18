using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class PatientBreedDescription : StringPropertyFilter<Study, StudyEntry>
    {
        public PatientBreedDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientBreedDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.PatientBreedDescription == null
                        || study.PatientBreedDescription == ""
                        || study.PatientBreedDescription == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.PatientBreedDescription == null
                        || study.PatientBreedDescription == ""
                       || SqlMethods.Like(study.PatientBreedDescription, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientBreedDescription);
        }
    }
 
    internal class PatientBreedCodeSequence
    {
        internal class CodingSchemeDesignator : StringPropertyFilter<Study, StudyEntry>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodingSchemeDesignator == null
                            || study.PatientBreedCodeSequenceCodingSchemeDesignator == ""
                            || study.PatientBreedCodeSequenceCodingSchemeDesignator == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodingSchemeDesignator == null
                            || study.PatientBreedCodeSequenceCodingSchemeDesignator == ""
                           || SqlMethods.Like(study.PatientBreedCodeSequenceCodingSchemeDesignator, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodingSchemeDesignator);
            }
        }

        internal class CodeValue : StringPropertyFilter<Study, StudyEntry>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodeValue == null
                            || study.PatientBreedCodeSequenceCodeValue == ""
                            || study.PatientBreedCodeSequenceCodeValue == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodeValue == null
                            || study.PatientBreedCodeSequenceCodeValue == ""
                           || SqlMethods.Like(study.PatientBreedCodeSequenceCodeValue, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodeValue);
            }
        }

        internal class CodeMeaning : StringPropertyFilter<Study, StudyEntry>
        {
            public CodeMeaning(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientBreedCodeSequence, DicomTags.CodeMeaning), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodeMeaning == null
                            || study.PatientBreedCodeSequenceCodeMeaning == ""
                            || study.PatientBreedCodeSequenceCodeMeaning == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is.
                return from study in query
                       where study.PatientBreedCodeSequenceCodeMeaning == null
                            || study.PatientBreedCodeSequenceCodeMeaning == ""
                           || SqlMethods.Like(study.PatientBreedCodeSequenceCodeMeaning, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientBreedCodeSequenceCodeMeaning);
            }
        }
    }  
}
