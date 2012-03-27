using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    public class WorkItemServiceProvider : IDuplexServiceProvider
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
    }
}
