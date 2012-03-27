#if UNIT_TESTS

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class WorkItemActivityMonitorTestApplication : IApplicationRoot
    {
        private IWorkItemActivityMonitor _workItemActivityMonitor;

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            Console.WriteLine("Starting WorkItemActivityMonitor test application ...");

            _workItemActivityMonitor = WorkItemActivityMonitor.Create(false);
            _workItemActivityMonitor.IsConnectedChanged += OnIsConnectedChanged;
            //_workItemActivityMonitor.WorkItemChanged += OnWorkItemChanged;

            Console.WriteLine("Press <Enter> to terminate.");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("IsConnected={0}");

            Console.ReadLine();

            _workItemActivityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
            //_workItemActivityMonitor.WorkItemChanged -= OnWorkItemChanged;
            _workItemActivityMonitor.Dispose();
        }

        #endregion

        private void OnWorkItemChanged(object sender, WorkItemChangedEventArgs e)
        {
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