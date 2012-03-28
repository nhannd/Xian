using System;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class WorkItemServiceProvider : IServiceProvider, IDuplexServiceProvider
    {
        #region Implementation of IDuplexServiceProvider

        public object GetService(Type type, object callback)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemService))
                return null;

            Platform.CheckExpectedType(callback, typeof(IWorkItemActivityCallback));
            
            var client = new WorkItemServiceClient(new InstanceContext(callback));
            if (client.State != CommunicationState.Opened)
                client.Open();

            return client;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return GetService(serviceType, WorkItemActivityCallback.Nil);
        }

        #endregion
    }
}
