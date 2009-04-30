#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
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
