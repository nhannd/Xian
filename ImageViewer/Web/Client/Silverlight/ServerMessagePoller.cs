#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using ClearCanvas.Web.Client.Silverlight;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    internal class MessagePollerEventReceivedEventArgs : EventArgs
    {
        public EventSet EventSet { get; set; }
    }
    internal class MessagePollerErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }

    internal class ServerMessagePoller : IDisposable
    {
        const int MinPollDelaySinceLastActivity = 100; // 100 ms since last activity

        private ApplicationServiceClient _service;
        private Thread _pollingThread;
        private bool _stop;
        private object _syncLock = new object();
        private long _lastPollTick = Environment.TickCount;
        private int _pendingPollingCount;

        public event EventHandler<MessagePollerEventReceivedEventArgs> MessageReceived;
        
        public ServerMessagePoller(ApplicationServiceClient service)
        {
            _service = service;
            _service.GetPendingEventCompleted += OnGetPendingEventCompleted;
        }

        public void Start()
        {
            _pollingThread = new Thread(ThreadStart);
            _pollingThread.Start();
        }

        private void ThreadStart(object ignore)
        {
            while (true)
            {
                if (_stop || _service.InnerChannel.State != System.ServiceModel.CommunicationState.Opened)
                {
                    return;
                }

                if (ApplicationContext.Current == null)
                {
                    Thread.Sleep(50);
                    continue;
                }

                long now = Environment.TickCount;

                // TimeSpan used to deal with roll over of TickCount
                // Note: Environment.TickCount unit is in ms
                if (TimeSpan.FromMilliseconds(now - ApplicationActivityMonitor.Instance.LastActivityTick) < TimeSpan.FromMilliseconds(MinPollDelaySinceLastActivity))
                {
                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, 50);
                        continue;
                    }
                }

                if (!DoPoll())
                {
                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, 50);
                    }
                }
            }
        }

        private void OnGetPendingEventCompleted(object sender, GetPendingEventCompletedEventArgs e)
        {
            _lastPollTick = Environment.TickCount;
            Interlocked.Decrement(ref _pendingPollingCount);

            lock (_syncLock)
            {
                Monitor.PulseAll(_syncLock);
            }

            if (e.Error != null)
            {
                ErrorHandler.HandleException(e.Error);
            }
            else
            {
                if (e.Result != null)
                {
                    if (e.Result.EventSet != null)
                    {
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessagePollerEventReceivedEventArgs { EventSet = e.Result.EventSet });
                        }
                    }
                }
            }
        }

        private bool DoPoll()
        {
            if (_pendingPollingCount == 0 && _service != null)
            {
                Interlocked.Increment(ref _pendingPollingCount);

                int maxWaitTime = 10000; //ms

                try
                {
                    
                    // TODO: the client may have disconnected
                    // Note: do not call this inside a lock statement. It can cause deadlock
                    _service.GetPendingEventAsync(new GetPendingEventRequest() { ApplicationId = ApplicationContext.Current.ID, MaxWaitTime = maxWaitTime });

                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, maxWaitTime - 100); // -100 so that another one will go out while the prev one is coming back. -100 = RTT/2
                    } 
                }
                catch (Exception x)
                {
                    // catch exception to prevent crashing
                    Platform.Log(LogLevel.Error, x, "Unexpected exception polling for server data");
                }
                finally
                {
                    _lastPollTick = Environment.TickCount;
                }
                
                return true;
            }

            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _stop = true;
            if (_pollingThread != null)
            {
                _service.GetPendingEventCompleted -= OnGetPendingEventCompleted;

                lock (_syncLock)
                    Monitor.PulseAll(_syncLock);
            }
        }

        #endregion
    }

}
