#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using NUnit.Framework;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ProcedureCheckInTests
	{
		# region Public Operations Tests

		[Test]
		public void Test_CheckIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();

			Assert.IsTrue(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);

			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckIn_AlreadyCheckedIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckIn(now);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckOut_NeverCheckedIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckOut(now);
		}

		[Test]
		public void Test_CheckOut_Typical()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);

			pc.CheckOut(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsTrue(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckOutTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckOut_AlreadyCheckedOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckOut(now);

			pc.CheckOut(now);
		}

		[Test]
		public void Test_RevertCheckIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckInTime);

			pc.RevertCheckIn();

			Assert.IsTrue(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.IsNull(pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_RevertCheckIn_NeverCheckedIn()
		{
			var pc = new ProcedureCheckIn();
			pc.RevertCheckIn();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_RevertCheckIn_AlreadyCheckedOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckOut(now + TimeSpan.FromSeconds(100));
			pc.RevertCheckIn();
		}

		#endregion
	}
}

#endif