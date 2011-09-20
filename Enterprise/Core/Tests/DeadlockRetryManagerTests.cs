#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Enterprise.Core.Tests
{
	[TestFixture]
	public class DeadlockRetryManagerTests
	{
		[Test]
		public void Test_default_property_values_set()
		{
			var d = new DeadlockRetryManager();
			Assert.AreEqual(3, d.MaxAttempts);
			Assert.AreEqual(100, d.MinWaitTime);
			Assert.AreEqual(500, d.MaxWaitTime);
		}

		[Test]
		public void Test_no_failure()
		{
			var count = 0;
			using(var d = new DeadlockRetryManager())
			{
				d.Execute(delegate
				          {
				          	count++;
				          });
			}

			// exactly 1 execution
			Assert.AreEqual(1, count);
		}

		[Test]
		public void Test_fail_1x_with_DeadlockException()
		{
			var count = 0;
			using (var d = new DeadlockRetryManager())
			{
				d.Execute(delegate
				{
					count++;
					if (count <= 1)
						throw new DeadlockException();
				});
			}

			// exactly 2 execution
			Assert.AreEqual(2, count);
		}

		[Test]
		public void Test_fail_2x_with_DeadlockException()
		{
			var count = 0;
			using (var d = new DeadlockRetryManager())
			{
				d.Execute(delegate
				{
					count++;
					if (count <= 2)
						throw new DeadlockException();
				});
			}

			// exactly 3 execution
			Assert.AreEqual(3, count);
		}

		[Test]
		[ExpectedException(typeof(DeadlockException))]
		public void Test_fail_3x_with_DeadlockException()
		{
			var count = 0;
			using (var d = new DeadlockRetryManager() {MaxAttempts = 3})
			{
				d.Execute(delegate
				{
					count++;
					if (count <= 3)
						throw new DeadlockException();
				});
			}
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void Test_fail_with_other_exception_does_not_retry()
		{
			using (var d = new DeadlockRetryManager() { MaxAttempts = 3 })
			{
				d.Execute(delegate
				{
					// only a DeadlockException will cause a retry
					throw new Exception();
				});
			}
		}
	}
}

#endif
