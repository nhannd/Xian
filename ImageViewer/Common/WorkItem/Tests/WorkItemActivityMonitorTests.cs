#if UNIT_TESTS

using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;
using System.Threading;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [TestFixture]
    public class WorkItemActivityMonitorTests : IWorkItemActivityCallback
    {
        private readonly object _syncLock = new object();

        private bool _gotIsConnectedChanged;
        private int _workItemChanged1Count;
        private int _workItemChanged2Count;
        private int _workItemChanged3Count;
        private int _workItemChangedCallbackCount;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var factory = new UnitTestExtensionFactory { { typeof(DuplexServiceProviderExtensionPoint), typeof(TestServiceProvider) } };
            Platform.SetExtensionFactory(factory);
        }

        private bool GotIsConnectedChanged
        {
            get { lock (_syncLock) { return _gotIsConnectedChanged; } }
            set { lock (_syncLock) { _gotIsConnectedChanged = value; } }
        }

        private int WorkItemChanged1Count
        {
            get { lock (_syncLock) { return _workItemChanged1Count; } }
            set { lock (_syncLock) { _workItemChanged1Count = value; } }
        }

        private int WorkItemChanged2Count
        {
            get { lock (_syncLock) { return _workItemChanged2Count; } }
            set { lock (_syncLock) { _workItemChanged2Count = value; } }
        }

        private int WorkItemChanged3Count
        {
            get { lock (_syncLock) { return _workItemChanged3Count; } }
            set { lock (_syncLock) { _workItemChanged3Count = value; } }
        }
        
        private int WorkItemChangedCallbackCount
        {
            get { lock (_syncLock) { return _workItemChangedCallbackCount; } }
            set { lock (_syncLock) { _workItemChangedCallbackCount = value; } }
        }

        [Test]
        public void TestEventProxyKeyEquality()
        {
            var key1 = new WorkItemChangedEventProxyKey(null, WorkItemChanged1);
            var key2 = new WorkItemChangedEventProxyKey(null, WorkItemChanged1);

            Assert.AreEqual(key1, key2);
            key2 = new WorkItemChangedEventProxyKey(WorkItemTypeEnum.DicomSend, WorkItemChanged1);

            Assert.AreNotEqual(key1, key2);
            key1 = new WorkItemChangedEventProxyKey(WorkItemTypeEnum.DicomSend, WorkItemChanged1);

            Assert.AreEqual(key1, key2);

            key2 = new WorkItemChangedEventProxyKey(WorkItemTypeEnum.DicomSend, WorkItemChanged2);
            Assert.AreNotEqual(key1, key2);
        }

        [Test]
        public void TestWorkItemChangedEventWrappers()
        {
            var wrappers = new WorkItemChangedEventWrappers();
            Assert.AreSame(wrappers.AllTypesWrapper, wrappers[null]);

            Assert.IsFalse(wrappers[null].IsActive);
            wrappers[null].IsSubscribedToService = true;

            Assert.IsTrue(wrappers[null].IsActive);
            Assert.IsFalse(wrappers[null].ShouldSubscribeToService);
            Assert.IsTrue(wrappers[null].ShouldUnsubscribeFromService);

            wrappers[null].Changed += WorkItemChanged1;
            Assert.IsTrue(wrappers[null].IsActive);
            Assert.IsFalse(wrappers[null].ShouldSubscribeToService);
            Assert.IsFalse(wrappers[null].ShouldUnsubscribeFromService);

            wrappers[null].IsSubscribedToService = false;
            Assert.IsTrue(wrappers[null].IsActive);
            Assert.IsTrue(wrappers[null].ShouldSubscribeToService);
            Assert.IsFalse(wrappers[null].ShouldUnsubscribeFromService);

            Assert.AreEqual(1, wrappers.GetActiveWrappers().Count());

            wrappers[null].Changed += WorkItemChanged2;
            Assert.IsTrue(wrappers[null].IsActive);
            Assert.IsTrue(wrappers[null].ShouldSubscribeToService);
            Assert.IsFalse(wrappers[null].ShouldUnsubscribeFromService);

            wrappers[null].Changed -= WorkItemChanged1;

            Assert.IsTrue(wrappers[null].IsActive);
            Assert.IsTrue(wrappers[null].ShouldSubscribeToService);
            Assert.IsFalse(wrappers[null].ShouldUnsubscribeFromService);

            wrappers[null].Changed -= WorkItemChanged2;

            Assert.IsFalse(wrappers[null].IsActive);
            Assert.IsFalse(wrappers[null].ShouldSubscribeToService);
            Assert.IsFalse(wrappers[null].ShouldUnsubscribeFromService);

            Assert.AreEqual(0, wrappers.GetActiveWrappers().Count());

            wrappers[null].IsSubscribedToService = true;
            wrappers[WorkItemTypeEnum.DicomSend].IsSubscribedToService = true;

            Assert.AreEqual(2, wrappers.GetActiveWrappers().Count());

            //TODO (Marmot): More tests on the Get* methods.
        }

        [Test]
        public void TestActivityMonitorProxyConnected()
        {
            var test = new TestActivityMonitor();
            var proxy = new WorkItemActivityMonitorProxy(test, null);

            GotIsConnectedChanged = false;

            test.IsConnected = true;
            Assert.IsFalse(_gotIsConnectedChanged);

            proxy.IsConnectedChanged += OnIsConnectedChanged;
            test.IsConnected = true;

            Assert.IsFalse(_gotIsConnectedChanged);

            test.IsConnected = false;
            Assert.IsTrue(_gotIsConnectedChanged);

            _gotIsConnectedChanged = false;
            test.IsConnected = true;
            Assert.IsTrue(_gotIsConnectedChanged);

            _gotIsConnectedChanged = false;
            proxy.IsConnectedChanged -= OnIsConnectedChanged;
            test.IsConnected = false;
            Assert.IsFalse(_gotIsConnectedChanged);
        }

        [Test]
        public void TestActivityMonitorProxyChanged()
        {
            ResetCallbackFields();

            var test = new TestActivityMonitor();
            var proxy = new WorkItemActivityMonitorProxy(test, null);

            var item = new WorkItemData {Type = WorkItemTypeEnum.DicomRetrieve};
            proxy.Subscribe(null, WorkItemChanged1);

            test.OnWorkItemChanged(item);
            Assert.AreEqual(1, WorkItemChanged1Count);
            Assert.AreEqual(0, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            proxy.Subscribe(WorkItemTypeEnum.DicomRetrieve, WorkItemChanged2);
            test.OnWorkItemChanged(item);
            Assert.AreEqual(2, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            item.Type = WorkItemTypeEnum.DicomSend;
            test.OnWorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            proxy.Unsubscribe(null, WorkItemChanged1);
            test.OnWorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            proxy.Subscribe(WorkItemTypeEnum.DicomSend, WorkItemChanged1);
            proxy.Subscribe(WorkItemTypeEnum.DicomSend, WorkItemChanged3);
            test.OnWorkItemChanged(item);
            Assert.AreEqual(4, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(1, WorkItemChanged3Count);
        }

        [Test]
        public void TestTestWorkItemService()
        {
            ResetCallbackFields();
            var service = new TestWorkItemService{Callback = this};

            var item = new WorkItemData { Type = WorkItemTypeEnum.DicomRetrieve };
            service.PublishWorkItemChanged(item);
            Assert.AreEqual(0, WorkItemChangedCallbackCount);

            service.Subscribe(new WorkItemSubscribeRequest {Type = null});

            service.PublishWorkItemChanged(item);
            Assert.AreEqual(1, WorkItemChangedCallbackCount);

            service.Subscribe(new WorkItemSubscribeRequest { Type = WorkItemTypeEnum.DicomRetrieve });
            service.PublishWorkItemChanged(item);
            Assert.AreEqual(2, WorkItemChangedCallbackCount);

            service.Unsubscribe(new WorkItemUnsubscribeRequest {Type = null});
            service.PublishWorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChangedCallbackCount);

            item.Type = WorkItemTypeEnum.DicomSend;
            service.PublishWorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChangedCallbackCount);

            item.Type = WorkItemTypeEnum.DicomRetrieve;
            service.Unsubscribe(new WorkItemUnsubscribeRequest { Type = WorkItemTypeEnum.DicomRetrieve });
            service.PublishWorkItemChanged(item);

            item.Type = WorkItemTypeEnum.DicomSend;
            service.Subscribe(new WorkItemSubscribeRequest { Type = WorkItemTypeEnum.DicomSend});
            service.PublishWorkItemChanged(item);
            Assert.AreEqual(4, WorkItemChangedCallbackCount);
        }

        [Test]
        public void TestRealActivityMonitorConnection()
        {
            var monitor = new RealWorkItemActivityMonitor();
            TestRealActivityMonitorConnection(monitor);
            monitor.Dispose();
        }

        [Test]
        public void TestRealActivityMonitorConnectionWithProxy()
        {
            var monitor = WorkItemActivityMonitor.Create();
            TestRealActivityMonitorConnection(monitor);
            monitor.Dispose();
            //TODO: assert proxy count?
        }

        private void TestRealActivityMonitorConnection(IWorkItemActivityMonitor monitor)
        {
            ResetCallbackFields();

            Assert.AreEqual(CommunicationState.Opened, TestServiceProvider.CurrentService.State);
            Assert.IsTrue(monitor.IsConnected);

            monitor.IsConnectedChanged += OnIsConnectedChanged;

            WaitForEvent(() => TestServiceProvider.CurrentService.Close());
            Assert.IsTrue(GotIsConnectedChanged);
            Assert.IsFalse(monitor.IsConnected);

            GotIsConnectedChanged = false;
            WaitForEvent(() => TestServiceProvider.CurrentService.Open());
            Assert.IsTrue(GotIsConnectedChanged);
            Assert.IsTrue(monitor.IsConnected);

            monitor.IsConnectedChanged -= OnIsConnectedChanged;
            GotIsConnectedChanged = false;
            WaitForEvent(() => TestServiceProvider.CurrentService.Close());
            Assert.IsFalse(GotIsConnectedChanged);
            Assert.IsFalse(monitor.IsConnected);
        }

        private void ResetCallbackFields()
        {
            lock (_syncLock)
            {
                _gotIsConnectedChanged = false;
                _workItemChanged1Count = 0;
                _workItemChanged2Count = 0;
                _workItemChanged3Count = 0;
                _workItemChangedCallbackCount = 0;
            }
        }

        private void WaitForEvent(Action trigger)
        {
            lock (_syncLock)
            {
                trigger();
                Monitor.Wait(_syncLock, TimeSpan.FromSeconds(10));
            }
        }

        #region IWorkItemActivityCallback Members

        void IWorkItemActivityCallback.WorkItemChanged(WorkItemData workItemData)
        {
            lock (_syncLock)
            {
                ++_workItemChangedCallbackCount;
                Monitor.Pulse(_syncLock);
            }
        }

        #endregion

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _gotIsConnectedChanged = true;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged1(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged1Count;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged2(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged2Count;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged3(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged3Count;
                Monitor.Pulse(_syncLock);
            }
        }
    }
}

#endif