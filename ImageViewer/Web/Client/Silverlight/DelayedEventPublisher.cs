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
using System.Windows.Threading;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class DelayedEventPublisher<TEventArgs> : IDisposable
        where TEventArgs:EventArgs
    {
        private DispatcherTimer _timer = null;
        private EventHandler<TEventArgs> _eventHandler;
        private object _lastSender;
        private TEventArgs _lastEvent;

        public DelayedEventPublisher(EventHandler<TEventArgs> eventHandler, TimeSpan timeout)
        {
            _timer = new DispatcherTimer() { Interval = timeout };
            _timer.Tick += OnTimeout;
            _eventHandler = eventHandler;
        }

        public DelayedEventPublisher(EventHandler<TEventArgs> eventHandler, double timeoutMilliseconds)
            : this(eventHandler, TimeSpan.FromMilliseconds(timeoutMilliseconds))
        {
        }

        public void Publish(object sender, TEventArgs @event)
        {
            lock (_timer)
            {
                _lastSender = sender;
                _lastEvent = @event;

                if (_timer.IsEnabled)
                {
                   //Logger.Write("Delayed event\n");
                    return; //ignore it
                }

                 _timer.Start();
            }
            
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                lock (_timer)
                {
                    PublishNow();
                }
            }
            
        }

        private void PublishNow()
        {
            if (_eventHandler == null || _timer==null)
                return;

            if (_timer.IsEnabled)
            {
                _timer.Stop();

                _eventHandler(_lastSender, _lastEvent);
            }
        }



        public void Dispose()
        {
            if (_timer != null)
            {
                lock (_timer)
                {
                    _timer.Stop();
                    _timer = null;
                }                
            }
        }
    }
}
