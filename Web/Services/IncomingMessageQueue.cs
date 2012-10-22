#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Web.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Web.Services
{
	internal class IncomingMessageQueue
	{
		public delegate void ProcessMessageSetDelegate(MessageSet messageSet);

		private readonly object _syncLock = new object();
		private readonly ProcessMessageSetDelegate _processMessageSet;
		private readonly Dictionary<int, MessageSet> _messageSets;

	    public IncomingMessageQueue(ProcessMessageSetDelegate processMessageSet)
		{
			_processMessageSet = processMessageSet;
			_messageSets = new Dictionary<int, MessageSet>();
			NextMessageSetNumber = 1;
		}

		internal int NextMessageSetNumber { get; private set; }

	    public int Count
	    {
            get
            {
                return _messageSets.Count;
            }
	    }

	    public bool ProcessMessageSet(MessageSet messageSet)
	    {
	        var ticks = Environment.TickCount;
	        foreach (var message in messageSet.Messages)
	            message.ReceiveTimeTicks = ticks;

            lock (_syncLock)
			{
			    if (messageSet.Number == NextMessageSetNumber)
				{
					ProcessPendingMessageSets(messageSet);
				    return true;
				}

                Platform.Log(LogLevel.Warn, "Received message set out of order (received:{0}, expected:{1})", messageSet.Number, NextMessageSetNumber);
                     
			    _messageSets[messageSet.Number] = messageSet;
			    return false;
			}
		}

		private void ProcessPendingMessageSets(MessageSet current)
		{
		    NextMessageSetNumber = current.Number + 1;
			_processMessageSet(current);

			while (_messageSets.TryGetValue(NextMessageSetNumber, out current))
			{
				_messageSets.Remove(NextMessageSetNumber);
				NextMessageSetNumber = current.Number + 1;
				_processMessageSet(current);
			}    
		}
	}
}
