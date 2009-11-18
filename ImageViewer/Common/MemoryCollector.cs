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

using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	public static partial class MemoryManager
	{
		private class MemoryCollector
		{
			private readonly object _waitObject = new object();
			private readonly TimeSpan _waitTimeout;

			public MemoryCollector(TimeSpan waitTimeout)
			{
				// -1 is acceptable because it's a special value for Infinite
				if (waitTimeout < TimeSpan.Zero && waitTimeout.TotalMilliseconds != -1)
					throw new ArgumentException("The specified timeout is less than zero.", "waitTimeout");

				_waitTimeout = waitTimeout;
			}

			public void Collect()
			{
				lock (_waitObject)
				{
					lock (_syncLock)
					{
						if (_collectionThread == null)
						{
							Platform.Log(LogLevel.Debug, "Collect called with no objects in the cache; returning immediately.");
							return;
						}
						else if (_waitTimeout == TimeSpan.Zero)
						{
							Platform.Log(LogLevel.Debug, "Collect called with zero wait time; returning immediately.");

							//don't wait if wait time is zero, just signal the collection
							Monitor.Pulse(_syncLock);
							return;
						}

						++_waitingClients;
						MemoryCollected += OnMemoryCollected;
						Monitor.Pulse(_syncLock);
					}

					Platform.Log(LogLevel.Debug, "Waiting for memory collection to complete.");

					Monitor.Wait(_waitObject, _waitTimeout);

					lock (_syncLock)
					{
						MemoryCollected -= OnMemoryCollected;
					}
				}
			}

			private void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
			{
				if (args.IsLast)
				{
					lock (_waitObject)
					{
						Platform.Log(LogLevel.Debug, "'Last' memory collection signal detected; releasing waiting thread.");
						Monitor.Pulse(_waitObject);
					}

					lock (_syncLock)
					{
						--_waitingClients;
					}
				}
			}
		}
	}
}
