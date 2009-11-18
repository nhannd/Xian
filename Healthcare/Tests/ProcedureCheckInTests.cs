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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
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
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);

            Assert.AreEqual(now, _procedureCheckIn.CheckInTime);    
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_CheckIn_ExistingCheckInTime()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);
            _procedureCheckIn.CheckIn(now);
        }

        [Test]
        public void Test_CheckOut()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckOut(now);

            Assert.AreEqual(now, _procedureCheckIn.CheckOutTime);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_CheckOut_ExistingCheckInTime()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckOut(now);
            _procedureCheckIn.CheckOut(now);
        }

        [Test]
        public void Test_IsPreCheckIn_True()
        {
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();

            Assert.IsTrue(_procedureCheckIn.IsPreCheckIn);
        }

        [Test]
        public void Test_IsPreCheckIn_False()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);

            Assert.IsFalse(_procedureCheckIn.IsPreCheckIn);
        }

        [Test]
        public void Test_IsCheckedIn()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);

            Assert.IsTrue(_procedureCheckIn.IsCheckedIn);
        }

        [Test]
        public void Test_IsCheckedIn_PreCheckInState()
        {
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();

            Assert.IsFalse(_procedureCheckIn.IsCheckedIn);
        }

        [Test]
        public void Test_IsCheckedIn_CheckedOutState()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);
            _procedureCheckIn.CheckOut(now + TimeSpan.FromSeconds(100));

            Assert.IsFalse(_procedureCheckIn.IsCheckedIn);
        }

        [Test]
        public void Test_IsCheckedOut()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckOut(now);

            Assert.IsTrue(_procedureCheckIn.IsCheckedOut);
        }

        [Test]
        public void Test_IsCheckedOut_False()
        {
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();

            Assert.IsFalse(_procedureCheckIn.IsCheckedOut);
        }

        [Test]
        public void Test_RevertCheckIn()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);
            _procedureCheckIn.RevertCheckIn();

            Assert.IsNull(_procedureCheckIn.CheckInTime);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_RevertCheckIn_IsCheckedout()
        {
            DateTime? now = DateTime.Now;
            ProcedureCheckIn _procedureCheckIn = new ProcedureCheckIn();
            _procedureCheckIn.CheckIn(now);
            _procedureCheckIn.CheckOut(now + TimeSpan.FromSeconds(100));
            _procedureCheckIn.RevertCheckIn();
        }

        #endregion
    }
}

#endif