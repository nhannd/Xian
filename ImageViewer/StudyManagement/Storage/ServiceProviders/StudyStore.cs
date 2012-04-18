using System;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    internal class StudyStoreQueryProxy : IStudyStoreQuery
    {
        #region IStudyStoreQuery Members

        public GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount(request.Criteria);
                return new GetStudyCountResult { StudyCount = count };
            }
        }

        public GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetStudyEntries(request.Criteria);
                return new GetStudyEntriesResult {StudyEntries = entries};
            }
        }

        public GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetSeriesEntries(request.Criteria);
                return new GetSeriesEntriesResult { SeriesEntries = entries };
            }
        }

        public GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var entries = context.GetStudyStoreQuery().GetImageEntries(request.Criteria);
                return new GetImageEntriesResult { ImageEntries = entries };
            }
        }

        #endregion
    }

    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class StudyStoreQueryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IStudyStoreQuery))
                return null;
            
            return new ProxyGenerator().CreateInterfaceProxyWithTargetInterface(
                typeof(IStudyStoreQuery), new StudyStoreQueryProxy()
                , new IInterceptor[] { new BasicFaultInterceptor() });
        }

        #endregion
    }
}
