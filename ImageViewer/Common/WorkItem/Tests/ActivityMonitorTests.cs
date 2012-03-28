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
    public class ActivityMonitorTests : IWorkItemActivityCallback
    {
        private readonly object _syncLock = new object();

        private int _isConnectedChangedCount;
        private int _workItemChanged1Count;
        private int _workItemChanged2Count;
        private int _workItemChanged3Count;
        private int _workItemChangedCallbackCount;

        private int _expectedAsyncEventCount;
        private volatile bool _expectingAsyncEvents = false;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var factory = new UnitTestExtensionFactory { { typeof(DuplexServiceProviderExtensionPoint), typeof(TestServiceProvider) } };
            Platform.SetExtensionFactory(factory);

            RealWorkItemActivityMonitor.ConnectionRetryInterval = TimeSpan.FromMilliseconds(100);
        }

        private int IsConnectedChangedCount
        {
            get { lock (_syncLock) { return _isConnectedChangedCount; } }
        }

        private int WorkItemChanged1Count
        {
            get { lock (_syncLock) { return _workItemChanged1Count; } }
        }

        private int WorkItemChanged2Count
        {
            get { lock (_syncLock) { return _workItemChanged2Count; } }
        }

        private int WorkItemChanged3Count
        {
            get { lock (_syncLock) { return _workItemChanged3Count; } }
        }
        
        private int WorkItemChangedCallbackCount
        {
            get { lock (_syncLock) { return _workItemChangedCallbackCount; } }
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
            ResetCallbackFields();
            var test = new TestActivityMonitor();
            var proxy = new WorkItemActivityMonitorProxy(test, null);

            test.IsConnected = true;
            Assert.AreEqual(0, IsConnectedChangedCount);

            proxy.IsConnectedChanged += OnIsConnectedChanged;
            test.IsConnected = true;

            Assert.AreEqual(0, IsConnectedChangedCount);

            test.IsConnected = false;
            Assert.AreEqual(1, IsConnectedChangedCount);

            test.IsConnected = true;
            Assert.AreEqual(2, IsConnectedChangedCount);

            proxy.IsConnectedChanged -= OnIsConnectedChanged;
            test.IsConnected = false;
            Assert.AreEqual(2, IsConnectedChangedCount);
        }

        [Test]
        public void TestActivityMonitorProxySubscriptions()
        {
            _expectingAsyncEvents = false;

            var testMonitor = new TestActivityMonitor();
            var monitorProxy = new WorkItemActivityMonitorProxy(testMonitor, null);

            TestActivityMonitorSubscriptions(monitorProxy, testMonitor);
        }

        [Test]
        public void TestTestWorkItemService()
        {
            ResetCallbackFields();
            var service = new TestWorkItemService{Callback = this};
            var callback = (IWorkItemActivityCallback) service;

            var item = new WorkItemData { Type = WorkItemTypeEnum.DicomRetrieve };
            
            callback.WorkItemChanged(item);
            Assert.AreEqual(0, WorkItemChangedCallbackCount);

            service.Subscribe(new WorkItemSubscribeRequest {Type = null});

            callback.WorkItemChanged(item);
            Assert.AreEqual(1, WorkItemChangedCallbackCount);

            service.Subscribe(new WorkItemSubscribeRequest { Type = WorkItemTypeEnum.DicomRetrieve });
            callback.WorkItemChanged(item);
            Assert.AreEqual(2, WorkItemChangedCallbackCount);

            service.Unsubscribe(new WorkItemUnsubscribeRequest {Type = null});
            callback.WorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChangedCallbackCount);

            item.Type = WorkItemTypeEnum.DicomSend;
            callback.WorkItemChanged(item);
            Assert.AreEqual(3, WorkItemChangedCallbackCount);

            item.Type = WorkItemTypeEnum.DicomRetrieve;
            service.Unsubscribe(new WorkItemUnsubscribeRequest { Type = WorkItemTypeEnum.DicomRetrieve });
            callback.WorkItemChanged(item);

            item.Type = WorkItemTypeEnum.DicomSend;
            service.Subscribe(new WorkItemSubscribeRequest { Type = WorkItemTypeEnum.DicomSend});
            callback.WorkItemChanged(item);
            Assert.AreEqual(4, WorkItemChangedCallbackCount);
        }

        [Test]
        public void TestRealActivityMonitorConnection()
        {
            _expectingAsyncEvents = true;
            //Make sure it's initially open.
            TestServiceProvider.ServiceInstance.Open();

            var monitor = new RealWorkItemActivityMonitor();
            TestActivityMonitorConnection(monitor);
            monitor.Dispose();
        }

        [Test]
        public void TestRealActivityMonitorConnectionWithProxy()
        {
            _expectingAsyncEvents = true;
            //Make sure it's initially open.
            TestServiceProvider.ServiceInstance.Open();

            var monitor = WorkItemActivityMonitor.Create(false);
            TestActivityMonitorConnection(monitor);
            monitor.Dispose();
            Assert.AreEqual(0, WorkItemActivityMonitor._proxyCount);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRealActivityMonitorConnectionWithProxy_FailCreate()
        {
            _expectingAsyncEvents = true;
            //Make sure it's initially open.
            TestServiceProvider.ServiceInstance.Open();

            var monitor = WorkItemActivityMonitor.Create();
            TestActivityMonitorConnection(monitor);
            monitor.Dispose();
            //TODO: assert proxy count?
        }

        [Test]
        public void TestRealActivityMonitorSubscriptions()
        {
            _expectingAsyncEvents = true;
            //Make sure it's initially open.
            TestServiceProvider.ServiceInstance.Open();

            var monitor = new RealWorkItemActivityMonitor();
            TestActivityMonitorSubscriptions(monitor, TestServiceProvider.ServiceInstance);
            monitor.Dispose();
        }

        [Test]
        public void TestRealActivityMonitorSubscriptionsWithProxy()
        {
            _expectingAsyncEvents = true;
            //Make sure it's initially open.
            TestServiceProvider.ServiceInstance.Open();

            var monitor = WorkItemActivityMonitor.Create(false);
            TestActivityMonitorSubscriptions(monitor, TestServiceProvider.ServiceInstance);
            monitor.Dispose();
            Assert.AreEqual(0, WorkItemActivityMonitor._proxyCount);
        }

        private void TestActivityMonitorConnection(IWorkItemActivityMonitor monitor)
        {
            ResetCallbackFields();

            Assert.AreEqual(CommunicationState.Opened, TestServiceProvider.ServiceInstance.State);
            Assert.IsTrue(monitor.IsConnected);

            monitor.IsConnectedChanged += OnIsConnectedChanged;

            WaitForEvent(() => TestServiceProvider.ServiceInstance.Close());
            Assert.AreEqual(1, IsConnectedChangedCount);
            Assert.IsFalse(monitor.IsConnected);

            WaitForEvent(() => TestServiceProvider.ServiceInstance.Open());
            Assert.AreEqual(2, IsConnectedChangedCount);
            Assert.IsTrue(monitor.IsConnected);

            monitor.IsConnectedChanged -= OnIsConnectedChanged;
            TestServiceProvider.ServiceInstance.Close();
            Thread.Sleep(1000); //should be more than enough time; we unsubscribed from the event, so there's nothing to wait for.
            Assert.AreEqual(2, IsConnectedChangedCount);
            Assert.IsFalse(monitor.IsConnected);
        }

        private void TestActivityMonitorSubscriptions(IWorkItemActivityMonitor monitor, IWorkItemActivityCallback callback)
        {
            ResetCallbackFields();

            var item = new WorkItemData { Type = WorkItemTypeEnum.DicomRetrieve };
            SubscribeAndPause(monitor, null, WorkItemChanged1);

            WaitForEvents(() => callback.WorkItemChanged(item), 1);
            Assert.AreEqual(1, WorkItemChanged1Count);
            Assert.AreEqual(0, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            SubscribeAndPause(monitor, WorkItemTypeEnum.DicomRetrieve, WorkItemChanged2);
            WaitForEvents(() => callback.WorkItemChanged(item), 2);
            Assert.AreEqual(2, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            item.Type = WorkItemTypeEnum.DicomSend;
            WaitForEvents(() => callback.WorkItemChanged(item), 1);
            Assert.AreEqual(3, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            UnsubscribeAndPause(monitor, null, WorkItemChanged1);
            Assert.AreEqual(3, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            SubscribeAndPause(monitor, WorkItemTypeEnum.DicomSend, WorkItemChanged1);
            SubscribeAndPause(monitor, WorkItemTypeEnum.DicomSend, WorkItemChanged3);
            WaitForEvents(() => callback.WorkItemChanged(item), 2);
            Assert.AreEqual(4, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(1, WorkItemChanged3Count);
        }

        private void SubscribeAndPause(IWorkItemActivityMonitor monitor, WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            monitor.Subscribe(workItemType, eventHandler);
            //It may take a sec for the monitor to subscribe via the actual service.
            Thread.Sleep(100);
        }

        private void UnsubscribeAndPause(IWorkItemActivityMonitor monitor, WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            monitor.Unsubscribe(workItemType, eventHandler);
            //It may take a sec for the monitor to unsubscribe via the actual service.
            Thread.Sleep(100);
        }

        private void ResetCallbackFields()
        {
            lock (_syncLock)
            {
                _isConnectedChangedCount = 0;
                _workItemChanged1Count = 0;
                _workItemChanged2Count = 0;
                _workItemChanged3Count = 0;
                _workItemChangedCallbackCount = 0;

                _expectedAsyncEventCount = 0;
            }
        }

        private void WaitForEvents(Action trigger, int count)
        {
            if (!_expectingAsyncEvents)
            {
                trigger();
                return;
            }

            lock (_syncLock)
            {
                _expectedAsyncEventCount = count;
                trigger();
                while (_expectedAsyncEventCount > 0)
                    Monitor.Wait(_syncLock);
            }
        }

        private void WaitForEvent(Action trigger)
        {
            WaitForEvents(trigger, 1);
        }

        #region IWorkItemActivityCallback Members

        void IWorkItemActivityCallback.WorkItemChanged(WorkItemData workItemData)
        {
            lock (_syncLock)
            {
                ++_workItemChangedCallbackCount;
                --_expectedAsyncEventCount;
                Monitor.Pulse(_syncLock);
            }
        }

        #endregion

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                ++_isConnectedChangedCount;
                --_expectedAsyncEventCount;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged1(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged1Count;
                --_expectedAsyncEventCount;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged2(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged2Count;
                --_expectedAsyncEventCount;
                Monitor.Pulse(_syncLock);
            }
        }

        private void WorkItemChanged3(object sender, WorkItemChangedEventArgs e)
        {
            lock (_syncLock)
            {
                ++_workItemChanged3Count;
                --_expectedAsyncEventCount;
                Monitor.Pulse(_syncLock);
            }
        }
    }
}

#endif