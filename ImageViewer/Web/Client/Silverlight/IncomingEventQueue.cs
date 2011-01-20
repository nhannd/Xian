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
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using System.ComponentModel;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Views;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    //TODO: Delete this?
	internal class IncomingEventQueue
	{
		private const long MAX_SIM_DELAY_MS = 50; // this delay actually blocks the UI thread so be careful

		public delegate void ProcessEventSetDelegate(EventSet eventSet);

		private readonly object _syncLock = new object();
		private readonly ProcessEventSetDelegate _processEventSet;
		private readonly Dictionary<int, EventSet> _eventSets;

		private PerformanceMonitor _performance = PerformanceMonitor.CurrentInstance;
        private long _timePrevEvent;
        

		public IncomingEventQueue(ProcessEventSetDelegate processEventSet)
		{
			_processEventSet = processEventSet;
			_eventSets = new Dictionary<int, EventSet>();
			NextEventSetNumber = 1;
		}

		internal int NextEventSetNumber { get; private set; }
		
		public void ProcessEventSet(EventSet eventSet)
		{
			lock (_syncLock)
			{
				if (eventSet.Number == NextEventSetNumber)
				{
					ProcessPendingEventSets(eventSet);
				}
				else
				{
					Logger.Write("Received event set out of order (received:{0}, expected:{1})", eventSet.Number, NextEventSetNumber);
					_eventSets[eventSet.Number] = eventSet;
				}
			}
		}

        
		private void ProcessPendingEventSets(EventSet current)
		{
			NextEventSetNumber = current.Number + 1;
            
            _processEventSet(current);

            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            _timePrevEvent = Environment.TickCount;

            List<Guid> tilesUpdated = new List<Guid>();
            foreach (Event e in current.Events)
            {
                if (e is TileUpdatedEvent)
                {
                    tilesUpdated.Add(e.SenderId);
                }
            }
			while (_eventSets.TryGetValue(NextEventSetNumber, out current))
			{
				_eventSets.Remove(NextEventSetNumber);
				NextEventSetNumber = current.Number + 1;
                _processEventSet(current);

                // TODO: REVIEW THIS
                // Per MSDN:
                // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                _timePrevEvent = Environment.TickCount;
			}
		}
	}
}
