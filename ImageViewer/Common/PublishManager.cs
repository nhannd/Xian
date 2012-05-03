#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

//2006 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

/// I have since rewritten the classes from scratch, but the design idea
/// is based on Juval Lowy's publish-subscribe framework.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	internal static class PublishManager<T>
		where T : class
	{
		private static readonly object _syncLock;
		private static readonly Dictionary<string, Dictionary<T, Queue<object[]>>> _pendingPublishItems;

		static PublishManager()
		{
			_syncLock = new object();
			_pendingPublishItems = new Dictionary<string, Dictionary<T, Queue<object[]>>>();
			foreach (string eventName in SubscriptionManager<T>.GetMethods())
				_pendingPublishItems.Add(eventName, new Dictionary<T, Queue<object[]>>());
		}

		public static void Publish(string eventName, params object[] args)
		{
			Debug.Assert(eventName != null && _pendingPublishItems.ContainsKey(eventName));

			Dictionary<T, Queue<object[]>> eventDictionary = _pendingPublishItems[eventName];
			T[] subscribers = SubscriptionManager<T>.GetSubscribers(eventName);

			lock (_syncLock)
			{
				foreach (T subscriber in subscribers)
				{
					//for each event, we need to ensure that only a single thread is publishing data
					//per subscriber at a time.  Otherwise, occasionally, you can end up with events 
					// reaching the subscribers in the wrong order.
					if (eventDictionary.ContainsKey(subscriber))
					{
						eventDictionary[subscriber].Enqueue(args);
					}
					else
					{
						Queue<object[]> queue = new Queue<object[]>();
						queue.Enqueue(args);
						eventDictionary.Add(subscriber, queue);
						//The Publish delegate called by the thread pool will keep re-adding itself to the thread pool 
						//with the same KeyValuePair until all data for the KeyValuePair (event-subscriber) has been published.
						ThreadPool.QueueUserWorkItem(Publish, new KeyValuePair<string, T>(eventName, subscriber));
					}
				}
			}
		}

		private static void Publish(object obj)
		{
			KeyValuePair<string, T> kvp = (KeyValuePair<string, T>)obj;
			object[] args;
			bool anyLeft = true;

			Dictionary<T, Queue<object[]>> eventDictionary = _pendingPublishItems[kvp.Key];

			lock (_syncLock)
			{
				args = eventDictionary[kvp.Value].Dequeue();

				if (eventDictionary[kvp.Value].Count == 0)
				{
					eventDictionary.Remove(kvp.Value);
					anyLeft = false;
				}
			}

			try
			{
				typeof(T).GetMethod(kvp.Key).Invoke(kvp.Value, args);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			if (anyLeft)
				ThreadPool.QueueUserWorkItem(Publish, kvp);
		}
	}
}
