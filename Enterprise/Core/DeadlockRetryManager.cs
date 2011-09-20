#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Encapsulates logic for re-trying a given block of code in the event of a <see cref="DeadlockException"/>.
	/// </summary>
	class DeadlockRetryManager : IDisposable
	{
		public delegate void Action();

		/// <summary>
		/// Gets or sets the maximum number of attempts that will be made before giving up.
		/// </summary>
		public int MaxAttempts { get; set; }

		/// <summary>
		/// Gets or sets the minimum wait time in milliseconds between retries.
		/// </summary>
		public int MinWaitTime { get; set; }

		/// <summary>
		/// Gets or sets the maximum wait time in milliseconds between retries.
		/// </summary>
		public int MaxWaitTime { get; set; }

		/// <summary>
		/// Gets the number of attempts already made.
		/// </summary>
		public int Attempts { get; private set; }

		private readonly Random _random;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DeadlockRetryManager()
		{
			_random = new Random();

			// set defaults
			MaxAttempts = 3;
			MinWaitTime = 100;
			MaxWaitTime = 500;
		}

		/// <summary>
		/// Executes the specified action, re-trying in the event of a <see cref="DeadlockException"/>.
		/// </summary>
		/// <remarks>
		/// The specified action must retryable - that is, it must be formulated such that it can be executed
		/// repeatedly in the event that it encounters a deadlock situation.  If a deadlock situation is
		/// encountered, the action must throw a <see cref="DeadlockException"/>.
		/// </remarks>
		/// <param name="action"></param>
		public void Execute(Action action)
		{
			while (true)
			{
				try
				{
					action();

					// success, break out of retry loop
					break;
				}
				catch (DeadlockException)
				{
					Attempts++;

					if (ShouldRetry)
					{
						Platform.Log(LogLevel.Warn, "Deadlock detected - will retry operation.");
						WaitRandom();
					}
					else
					{
						// rethrow
						throw;
					}
				}
			}
		}

		public void Dispose()
		{
		}


		private bool ShouldRetry
		{
			get { return Attempts < MaxAttempts; }
		}

		private void WaitRandom()
		{
			// wait a random amount of time, so that other deadlock counterparts
			// don't retry at exactly the same instant
			Thread.Sleep(_random.Next(MinWaitTime, MaxWaitTime));
		}
	}
}
