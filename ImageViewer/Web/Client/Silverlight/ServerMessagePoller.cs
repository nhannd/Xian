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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    internal class MessagePollerEventReceivedEventArgs:EventArgs
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

        private object _sync = new object();


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


                // TODO: REVIEW THIS
                // Per MSDN:
                // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                long now = Environment.TickCount;
                if (now - ApplicationActivityMonitor.Instance.LastActivityTick < MinPollDelaySinceLastActivity)
                {
                    Thread.Sleep(50);
                    continue;
                }

                if (!DoPoll())
                {
                    Thread.Sleep(50);
                }

            }
        }

        private void OnGetPendingEventCompleted(object sender, GetPendingEventCompletedEventArgs e)
        {
            _lastPollTick = Environment.TickCount;
            Interlocked.Decrement(ref _pendingPollingCount);

            lock (_sync)
            {
                Monitor.PulseAll(_sync);
            }


            if (e.Error != null)
            {
                ErrorHandler.HandleException(e.Error);
            }
            else
            {
                if (e.Result != null)
                {

                    if (e.Result.Events != null)
                    {
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessagePollerEventReceivedEventArgs { EventSet = e.Result });
                        }

                    }

                }
            }
        }

        private bool DoPoll()
        {
            if (_pendingPollingCount == 0 && _service != null)
            {
                lock (_sync)
                {
                    Interlocked.Increment(ref _pendingPollingCount);

                    int maxWaitTime = 10000;

                    try
                    {
                        // TODO: the client may have disconnected
                        _service.GetPendingEventAsync(new GetPendingEventRequest() { ApplicationId = ApplicationContext.Current.ID, MaxWaitTime = maxWaitTime });

                        Monitor.Wait(_sync, maxWaitTime - 100); // -100 so that another one will go out while the prev one is coming back. -100 = RTT/2
                    }
                    catch (Exception)
                    {
                        // catch exception to prevent crashing
                    }
                    finally
                    {
                        

                        // TODO: REVIEW THIS
                        // Per MSDN:
                        // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                        // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                        _lastPollTick = Environment.TickCount;
                    }
                }
                return true;
            }

            return false;

        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_pollingThread != null)
            {
                _stop = true;
                _service.GetPendingEventCompleted -= OnGetPendingEventCompleted;
            }
        }

        #endregion
    }

}
