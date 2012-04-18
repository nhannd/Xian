using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    //internal class BitsAllocatedInStudy : StringPropertyFilter<Study, StudyEntry>
    //{
    //    public BitsAllocatedInStudy(StudyEntry criteria)
    //    {
    //        AddToQueryEnabled = true;
    //        FilterResultsEnabled = true;
    //    }

    //    protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
    //    {
    //        //DICOM says for any keys (required or optional) that we support matching on will always consider
    //        //an empty value to be a match regardless of what the criteria is.
    //        return from study in query
    //               where study.SopClassesInStudy == null
    //                    || study.SopClassesInStudy == ""
    //                    || study.SopClassesInStudy == criterion
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"{0}\%", criterion))
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}", criterion))
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}\%", criterion))
    //               select study;
    //    }

    //    protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
    //    {
    //        //DICOM says for any keys (required or optional) that we support matching on will always consider
    //        //an empty value to be a match regardless of what the criteria is.
    //        return from study in query
    //               where study.SopClassesInStudy == null
    //                    || study.SopClassesInStudy == ""
    //                    || SqlMethods.Like(study.SopClassesInStudy, criterion)
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"{0}\%", criterion))
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}", criterion))
    //                    || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%{0}\%", criterion))
    //               select study;
    //    }

    //    protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
    //    {
    //        resultAttribute.SetStringValue(item.SopClassesInStudy);
    //    }
    //}
}
