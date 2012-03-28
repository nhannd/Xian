#if UNIT_TESTS

using System;
using ClearCanvas.Common;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    internal class TestServiceProvider : IDuplexServiceProvider
    {
        internal static readonly TestWorkItemService ServiceInstance = new TestWorkItemService();

        #region Implementation of IDuplexServiceProvider

        public object GetService(Type type, object callback)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemActivityMonitorService))
                return null;

            Platform.CheckExpectedType(callback, typeof(IWorkItemActivityCallback));
            if (ServiceInstance.State != CommunicationState.Opened)
                throw new EndpointNotFoundException("Test service not running.");

            ServiceInstance.Callback = (IWorkItemActivityCallback)callback;
            return ServiceInstance;
        }

        #endregion
    }
}

#endif