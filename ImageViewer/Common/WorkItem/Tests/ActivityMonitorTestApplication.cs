#if UNIT_TESTS

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ActivityMonitorTestApplication : IApplicationRoot
    {
        private IWorkItemActivityMonitor _workItemActivityMonitor;

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            Console.WriteLine("Starting WorkItemActivityMonitor test application ...");

            _workItemActivityMonitor = WorkItemActivityMonitor.Create(false);
            _workItemActivityMonitor.IsConnectedChanged += OnIsConnectedChanged;
            _workItemActivityMonitor.WorkItemChanged += OnWorkItemChanged;
            
            Console.WriteLine("Press <Enter> to terminate.");
            Console.WriteLine();
            Console.WriteLine();

            string message = String.Format("IsConnected={0}", _workItemActivityMonitor.IsConnected);
            Console.WriteLine(message);

            Console.ReadLine();

            _workItemActivityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
            _workItemActivityMonitor.WorkItemChanged -= OnWorkItemChanged;
            _workItemActivityMonitor.Dispose();
        }

        #endregion

        private void OnWorkItemChanged(object sender, WorkItemChangedEventArgs e)
        {
            if (e.ItemData != null && e.ItemData.Request != null)
            {
                Console.WriteLine("Received WorkItemChanged event: {0}:{1}.", e.ItemData.Request.ActivityType,
                                  e.ItemData.Request.ActivityDescription);
                if (e.ItemData.Progress != null)
                    Console.WriteLine("  Progress: {0}, Details: {1}.", e.ItemData.Progress.Status,
                        e.ItemData.Progress.StatusDetails);
            }
            else
                Console.WriteLine("Received WorkItemChanged event.");
        }

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            string message = String.Format("Received IsConnectedChanged event (value={0}).", _workItemActivityMonitor.IsConnected);
            Console.WriteLine(message);
        }
    }
}

#endif