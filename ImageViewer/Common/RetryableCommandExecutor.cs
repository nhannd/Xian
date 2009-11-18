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
		private class RetryableCommandExecutor
		{
			private readonly object _waitObject = new object();
			private bool _needMoreMemory;

			private readonly RetryableCommand _retryableCommand;
			private readonly TimeSpan _maxWaitTime;
			private DateTime _startTime;
			private TimeSpan _waitTimeRemaining;

			public RetryableCommandExecutor(RetryableCommand retryableCommand, TimeSpan maxWaitTime)
			{
				Platform.CheckForNullReference(retryableCommand, "retryableCommand");
				Platform.CheckNonNegative((int)maxWaitTime.TotalMilliseconds, "maxWaitTime");

				_maxWaitTime = maxWaitTime;
				_retryableCommand = retryableCommand;
				_needMoreMemory = true;
			}
			
			public void Execute()
			{
				_startTime = DateTime.Now;

				try
				{
					_retryableCommand();
				}
				catch (OutOfMemoryException)
				{
					UpdateWaitTimeRemaining();
					if (_waitTimeRemaining <= TimeSpan.Zero)
						throw;

					if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					{
						Platform.Log(LogLevel.Debug, "Detected out of memory condition; retrying for up to {0} seconds.",
								 _waitTimeRemaining.TotalSeconds);
					}

					RetryExecute();
				}
			}


			private void RetryExecute()
			{
				lock (_waitObject)
				{
					lock (_syncLock)
					{
						if (_collectionThread == null)
						{
							Platform.Log(LogLevel.Debug, "RetryExecute called with no objects in the cache; returning immediately.");
							return;
						}

						++_waitingClients;
						MemoryCollected += OnMemoryCollected;
						Monitor.Pulse(_syncLock);
					}
					
					try
					{
						DoRetryExecute();
					}
					finally
					{
						//do this no matter what the reason.
						_needMoreMemory = false;
						Monitor.Pulse(_waitObject);
					}
				}
			}

			private void DoRetryExecute()
			{
				int retryNumber = 0;
				while (true)
				{
					DateTime begin = DateTime.Now;
					++retryNumber;

					Platform.Log(LogLevel.Debug, "Waiting for memory collected signal before retrying command.");
					Monitor.Wait(_waitObject, _waitTimeRemaining);

					try
					{
						_retryableCommand();
						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							TimeSpan elapsed = DateTime.Now - begin;
							Platform.Log(LogLevel.Debug, "Retry #{0} succeeded and took {0} seconds.", retryNumber, elapsed.TotalSeconds);
						}

						return;
					}
					catch (OutOfMemoryException)
					{
						this.UpdateWaitTimeRemaining();
						if (_waitTimeRemaining <= TimeSpan.Zero)
						{
							Platform.Log(LogLevel.Debug,
								"Retry #{0} failed and timeout has expired; throwing OutOfMemoryException.", retryNumber);

							throw;
						}

						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							TimeSpan elapsed = DateTime.Now - begin;
							Platform.Log(LogLevel.Debug, "Retry #{0} failed and took {1} seconds; wait time remaining is {2} seconds.",
							retryNumber, elapsed.TotalSeconds, _waitTimeRemaining.TotalSeconds);
						}
					}
					finally
					{
						Monitor.Pulse(_waitObject);
					}
				}
			}

			private void UpdateWaitTimeRemaining()
			{
				TimeSpan elapsed = DateTime.Now - _startTime;
				_waitTimeRemaining = _maxWaitTime - elapsed;
				if (_waitTimeRemaining < TimeSpan.Zero)
					_waitTimeRemaining = TimeSpan.Zero;
			}

			private void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
			{
				lock (_waitObject)
				{
					Platform.Log(LogLevel.Debug, "Received memory collected signal; waiting to see if more memory is required.");
					Monitor.Pulse(_waitObject);
					if (_needMoreMemory)
					{
						Monitor.Wait(_waitObject);
						args.NeedMoreMemory = _needMoreMemory;
					}

					if (!_needMoreMemory)
					{
						lock (_syncLock)
						{
							--_waitingClients;
							MemoryCollected -= OnMemoryCollected;
						}
					}
				}
			}
		}
	}
}
