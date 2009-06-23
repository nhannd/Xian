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