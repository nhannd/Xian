using System;
using System.Collections.Generic;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    internal class StudyStoreQueryProxy : IStudyStore
    {
        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            using (var context = new DataAccessContext())
            {
                return context.GetStudyStoreQuery().StudyQuery(queryCriteria);
            }
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            using (var context = new DataAccessContext())
            {
                return context.GetStudyStoreQuery().SeriesQuery(queryCriteria);
            }
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            using (var context = new DataAccessContext())
            {
                return context.GetStudyStoreQuery().ImageQuery(queryCriteria);
            }
        }

        public GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            using (var context = new DataAccessContext())
            {
                return context.GetStudyStoreQuery().GetStudyCount(request);
            }
        }
    }

    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class StudyStoreServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IStudyStore))
                return null;
            
            return new ProxyGenerator().CreateInterfaceProxyWithTargetInterface(
                typeof(IStudyStore), new StudyStoreQueryProxy()
                , new IInterceptor[] { new BasicFaultInterceptor() });
        }

        #endregion
    }
}
