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
        //TODO (Marmot):Need to actually do something to test this.
        private int _studiesClearedCallbackCount;

        private int _expectedAsyncEventCount;
        private volatile bool _expectingAsyncEvents;

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

            service.Subscribe(new WorkItemSubscribeRequest());

            callback.WorkItemChanged(item);
            Assert.AreEqual(1, WorkItemChangedCallbackCount);

            service.Unsubscribe(new WorkItemUnsubscribeRequest {Type = null});
            callback.WorkItemChanged(item);
            Assert.AreEqual(1, WorkItemChangedCallbackCount);
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
            SubscribeAndPause(monitor, WorkItemChanged1);

            WaitForEvents(() => callback.WorkItemChanged(item), 1);
            Assert.AreEqual(1, WorkItemChanged1Count);
            Assert.AreEqual(0, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            monitor.WorkItemTypeFilters = new[] {WorkItemTypeEnum.DicomRetrieve};
            SubscribeAndPause(monitor, WorkItemChanged2);
            WaitForEvents(() => callback.WorkItemChanged(item), 2);
            Assert.AreEqual(2, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            item.Type = WorkItemTypeEnum.DicomSend;
            callback.WorkItemChanged(item);
            Thread.Sleep(100);
            Assert.AreEqual(2, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            UnsubscribeAndPause(monitor, WorkItemChanged1);
            callback.WorkItemChanged(item);
            Thread.Sleep(100);
            Assert.AreEqual(2, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(0, WorkItemChanged3Count);

            monitor.WorkItemTypeFilters = new[] { WorkItemTypeEnum.DicomSend};
            SubscribeAndPause(monitor, WorkItemChanged1);
            UnsubscribeAndPause(monitor, WorkItemChanged2);
            SubscribeAndPause(monitor, WorkItemChanged3);
            WaitForEvents(() => callback.WorkItemChanged(item), 2);
            Assert.AreEqual(3, WorkItemChanged1Count);
            Assert.AreEqual(1, WorkItemChanged2Count);
            Assert.AreEqual(1, WorkItemChanged3Count);

            //TODO (Marmot): Expand to include multiple filters, etc.
        }

        private void SubscribeAndPause(IWorkItemActivityMonitor monitor, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            monitor.WorkItemChanged += eventHandler;
            //It may take a sec for the monitor to subscribe via the actual service.
            Thread.Sleep(100);
        }

        private void UnsubscribeAndPause(IWorkItemActivityMonitor monitor, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            monitor.WorkItemChanged -= eventHandler;
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

        void IWorkItemActivityCallback.StudiesCleared()
        {
            lock (_syncLock)
            {
                ++_studiesClearedCallbackCount;
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