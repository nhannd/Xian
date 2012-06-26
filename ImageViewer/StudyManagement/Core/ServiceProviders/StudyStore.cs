using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class StudyStoreQueryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IStudyStoreQuery))
                return null;

            return new StudyStoreQueryProxy();
        }

        #endregion
    }

    internal class StudyStoreQueryProxy : IStudyStoreQuery
    {
        private GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount(request.Criteria);                
                return new GetStudyCountResult { StudyCount = count };
            }
        }

        private GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetStudyEntries(request.Criteria);

                var criteria = (request.Criteria ?? new StudyEntry()).Study ?? new StudyRootStudyIdentifier();
                AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
                    SopClass.StudyRootQueryRetrieveInformationModelFindUid, criteria.ToDicomAttributeCollection());
                
                return new GetStudyEntriesResult { StudyEntries = entries };
            }
        }

        private GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetSeriesEntries(request.Criteria);

                var criteria = (request.Criteria ?? new SeriesEntry()).Series ?? new SeriesIdentifier();
                AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
                    SopClass.StudyRootQueryRetrieveInformationModelFindUid, criteria.ToDicomAttributeCollection());

                return new GetSeriesEntriesResult { SeriesEntries = entries };
            }
        }

        private GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetImageEntries(request.Criteria);

                var criteria = (request.Criteria ?? new ImageEntry()).Image ?? new ImageIdentifier();
                AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
                    SopClass.StudyRootQueryRetrieveInformationModelFindUid, criteria.ToDicomAttributeCollection());

                return new GetImageEntriesResult { ImageEntries = entries };
            }
        }

        #region IStudyStoreQuery Members

        GetStudyCountResult IStudyStoreQuery.GetStudyCount(GetStudyCountRequest request)
        {
            return ServiceProxyHelper.Call(GetStudyCount, request);
        }

        GetStudyEntriesResult IStudyStoreQuery.GetStudyEntries(GetStudyEntriesRequest request)
        {
            return ServiceProxyHelper.Call(GetStudyEntries, request);
        }

        GetSeriesEntriesResult IStudyStoreQuery.GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            return ServiceProxyHelper.Call(GetSeriesEntries, request);
        }

        GetImageEntriesResult IStudyStoreQuery.GetImageEntries(GetImageEntriesRequest request)
        {
            return ServiceProxyHelper.Call(GetImageEntries, request);
        }

        #endregion
    }
}
