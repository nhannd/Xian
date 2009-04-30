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
using System.Reflection;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal static class SubscriptionManager<T>
		where T : class 
	{
		private static readonly object _syncLock;
		private static readonly Dictionary<string, List<T>> _subscribers;

		static SubscriptionManager()
		{
			_syncLock = new object();

			Type operationContractType = typeof (OperationContractAttribute);
			Type callbackType = typeof (T);
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

			_subscribers = new Dictionary<string, List<T>>();
			foreach (MethodInfo info in callbackType.GetMethods(bindingFlags))
			{
				if (info.IsDefined(operationContractType, false))
					_subscribers[info.Name] = new List<T>();
			}
		}

		public static IEnumerable<string> GetMethods()
		{
			foreach (string eventName in _subscribers.Keys)
				yield return eventName;
		}

		public static void Subscribe(string eventOperation)
		{
			Subscribe(OperationContext.Current.GetCallbackChannel<T>(), eventOperation);
		}

		public static void Subscribe(T callback, string eventOperation)
		{
			if (String.IsNullOrEmpty(eventOperation))
			{
				lock (_syncLock)
				{
					foreach (string subscribeMethod in _subscribers.Keys)
					{
						if (!_subscribers[subscribeMethod].Contains(callback))
							_subscribers[subscribeMethod].Add(callback);
					}
				}
			}
			else if (_subscribers.ContainsKey(eventOperation))
			{
				lock (_syncLock)
				{
					if (!_subscribers[eventOperation].Contains(callback))
						_subscribers[eventOperation].Add(callback);
				}
			}
			else
			{
				Debug.Assert(false);
			}
		}

		public static void Unsubscribe(string eventOperation)
		{
			Unsubscribe(OperationContext.Current.GetCallbackChannel<T>(), eventOperation);
		}

		public static void Unsubscribe(T callback, string eventOperation)
		{
			if (String.IsNullOrEmpty(eventOperation))
			{
				lock (_syncLock)
				{
					foreach (string method in _subscribers.Keys)
						_subscribers[method].Remove(callback);
				}
			}
			else if (_subscribers.ContainsKey(eventOperation))
			{
				lock (_syncLock)
				{
					_subscribers[eventOperation].Remove(callback);
				}
			}
		}

		public static T[] GetSubscribers(string eventOperation)
		{
			Debug.Assert(eventOperation != null && _subscribers.ContainsKey(eventOperation));

			lock (_syncLock)
			{
				return _subscribers[eventOperation].ToArray();
			}
		}
	}
}
