#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
	internal class IncomingEventQueue
	{
		public delegate void ProcessEventSetDelegate(EventSet eventSet);

		private readonly object _syncLock = new object();
		private readonly ProcessEventSetDelegate _processEventSet;
		private readonly Dictionary<int, EventSet> _eventSets;

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

			while (_eventSets.TryGetValue(NextEventSetNumber, out current))
			{
				_eventSets.Remove(NextEventSetNumber);
				NextEventSetNumber = current.Number + 1;
				_processEventSet(current);
			}
		}
	}
}
