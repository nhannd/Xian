#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
			pc.CheckIn(now);

			Assert.AreEqual(now, pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckIn_ExistingCheckInTime()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckIn(now);
		}

		[Test]
		public void Test_CheckOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckOut(now);

			Assert.AreEqual(now, pc.CheckOutTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckOut_ExistingCheckInTime()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckOut(now);
			pc.CheckOut(now);
		}

		[Test]
		public void Test_IsPreCheckIn_True()
		{
			var pc = new ProcedureCheckIn();

			Assert.IsTrue(pc.IsPreCheckIn);
		}

		[Test]
		public void Test_IsPreCheckIn_False()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
		}

		[Test]
		public void Test_IsCheckedIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsTrue(pc.IsCheckedIn);
		}

		[Test]
		public void Test_IsCheckedIn_PreCheckInState()
		{
			var pc = new ProcedureCheckIn();

			Assert.IsFalse(pc.IsCheckedIn);
		}

		[Test]
		public void Test_IsCheckedIn_CheckedOutState()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckOut(now + TimeSpan.FromSeconds(100));

			Assert.IsFalse(pc.IsCheckedIn);
		}

		[Test]
		public void Test_IsCheckedOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckOut(now);

			Assert.IsTrue(pc.IsCheckedOut);
		}

		[Test]
		public void Test_IsCheckedOut_False()
		{
			var pc = new ProcedureCheckIn();

			Assert.IsFalse(pc.IsCheckedOut);
		}

		[Test]
		public void Test_RevertCheckIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.RevertCheckIn();

			Assert.IsNull(pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_RevertCheckIn_IsCheckedout()
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