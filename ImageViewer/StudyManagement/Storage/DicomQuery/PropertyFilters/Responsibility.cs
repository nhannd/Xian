using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class ResponsiblePerson : StringDicomPropertyFilter<Study>
    {
        public ResponsiblePerson(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePerson), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsiblePerson == null
                        || study.ResponsiblePerson == ""
                        || study.ResponsiblePerson == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsiblePerson == null
                        || study.ResponsiblePerson == ""
                       || SqlMethods.Like(study.ResponsiblePerson, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePerson);
        }
    }

    internal class ResponsiblePersonRole : StringDicomPropertyFilter<Study>
    {
        public ResponsiblePersonRole(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePersonRole), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsiblePersonRole == null
                        || study.ResponsiblePersonRole == ""
                        || study.ResponsiblePersonRole == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsiblePersonRole == null
                        || study.ResponsiblePersonRole == ""
                       || SqlMethods.Like(study.ResponsiblePersonRole, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePersonRole);
        }
    }

    internal class ResponsibleOrganization : StringDicomPropertyFilter<Study>
    {
        public ResponsibleOrganization(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsibleOrganization), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsibleOrganization == null
                        || study.ResponsibleOrganization == ""
                        || study.ResponsibleOrganization == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is.
            return from study in query
                   where study.ResponsibleOrganization == null
                        || study.ResponsibleOrganization == ""
                       || SqlMethods.Like(study.ResponsibleOrganization, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsibleOrganization);
        }
    }
}