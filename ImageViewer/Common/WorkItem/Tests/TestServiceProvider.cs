using System;
using ClearCanvas.Common;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    internal class TestServiceProvider : IDuplexServiceProvider
    {
        internal static volatile TestWorkItemService CurrentService = new TestWorkItemService();

        #region Implementation of IDuplexServiceProvider

        public object GetService(Type type, object callback)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemService))
                return null;

            Platform.CheckExpectedType(callback, typeof(IWorkItemActivityCallback));
            CurrentService.Callback = (IWorkItemActivityCallback)callback;
            if (CurrentService.State != CommunicationState.Opened)
                throw new EndpointNotFoundException();

            return CurrentService;
        }

        #endregion
    }
}
