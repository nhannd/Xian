#if UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Workflow;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProcedureStepTests
    {
        #region ConcreteProcedureStep

        class ConcreteProcedureStep : ProcedureStep
        {
            public ConcreteProcedureStep()
            {
                
            }

            public ConcreteProcedureStep(Procedure procedure) 
                : base(procedure)
            {

            }

            public override string Name
            {
                get { return "Test string."; }
            }

            public override bool IsPreStep
            {
                get { return false; }
            }

            protected override void LinkProcedure(Procedure procedure)
            {
                // Do nothing
            }

            public override List<Procedure> GetLinkedProcedures()
            {
                return null;
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new ConcreteProcedureStep(Procedure);
            }

            protected override bool IsRelatedStep(ProcedureStep step)
            {
                return true;
            }
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            ProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Assert.AreEqual(procedure, procedureStep.Procedure);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(procedureStep));
        }
        
        #endregion

        #region Property Tests

        [Test]
        public void Test_AssignedStaff()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            procedureStep.Assign(performer);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNotNull(procedureStep.Scheduling.Performer);
            Assert.AreEqual(performer, procedureStep.AssignedStaff);
        }

        [Test]
        public void Test_AssignedStaff_SchedulingNull()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            Assert.IsFalse(procedureStep.Scheduling != null && procedureStep.Scheduling.Performer != null);
            Assert.IsNull(procedureStep.AssignedStaff);
        }

        [Test]
        public void Test_PerformingStaff()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            procedureStep.Start(performer);

            Assert.IsNotNull(procedureStep.Performer);
            Assert.AreEqual(performer, procedureStep.PerformingStaff);
        }

        [Test]
        public void Test_PerformingStaff_PerformerNull()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            Assert.IsNull(procedureStep.Performer);
            Assert.IsNull(procedureStep.PerformingStaff);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_Assign()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
            
            procedureStep.Assign(performer);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNotNull(procedureStep.Scheduling.Performer);
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), procedureStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Scheduling.Performer).Staff);
        }

        [Test]
        public void Test_Assign_NullStaff()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            // checking that assigning null has no effect if already null
            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
            procedureStep.Assign((Staff)null);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);

            // check that assigning null has the effect of un-assiging if already assigned
            Staff performer = new Staff();
            procedureStep.Assign(performer);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNotNull(procedureStep.Scheduling.Performer);

            procedureStep.Assign((Staff)null);
            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
        }

        [Test]
        public void Test_Reassign_Scheduled()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer); // Perform event

            // Make assertions
            Assert.IsNotNull(newStep.Scheduling);
            Assert.IsNotNull(newStep.Scheduling.Performer);
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), newStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)newStep.Scheduling.Performer).Staff);
        }

        [Test]
        public void Test_Reassign_InProgressOrSuspended()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            procedureStep.Schedule(now, end);
            procedureStep.Suspend();
            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer); // Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
            Assert.AreEqual(ActivityStatus.SC, newStep.State);
            Assert.AreEqual(now, newStep.Scheduling.StartTime);
            Assert.AreEqual(end, newStep.Scheduling.EndTime);
            Assert.IsNotNull(newStep.Scheduling);
            Assert.IsNotNull(newStep.Scheduling.Performer);
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), newStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)newStep.Scheduling.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Reassign_Terminated()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            procedureStep.Discontinue();

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer);
        }

        [Test]
        public void Test_Start()
        {
            DateTime? later = DateTime.Now.AddHours(2);
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer, later);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(later, procedureStep.StartTime);
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        public void Test_Start_NullStartTime()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer, null);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.StartTime));
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        public void Test_Start_NoStartTime()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.StartTime));
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Start_NullPerformer()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            procedureStep.Start((Staff)null); // Perform event
        }

        [Test]
        public void Test_Complete()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Complete(performer); // Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.EndTime));
            Assert.IsInstanceOfType(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Complete_NullPerformer()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            procedureStep.Complete((Staff)null); // Perform event
        }

        [Test]
        public void Test_LinkTo()
        {
            Procedure procedure = new Procedure();
            Procedure procedure2 = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            ConcreteProcedureStep procedureStep2 = new ConcreteProcedureStep(procedure2);
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);
            
            procedureStep.LinkTo(procedureStep2); // Perform event

            // Make assertions
            Assert.AreEqual(procedureStep2, procedureStep.LinkStep);
            Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_LinkTo_StateNotScheduling()
        {
            Procedure procedure = new Procedure();
            Procedure procedure2 = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            ConcreteProcedureStep procedureStep2 = new ConcreteProcedureStep(procedure2);
            procedureStep.Discontinue();

            procedureStep.LinkTo(procedureStep2); // Perform event
        }

        //[Test]
        //[ExpectedException(typeof(WorkflowException))]
        //public void Test_Linkprocedure()
        //{
        //    Procedure procedure = new Procedure();
        //    ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
        //    ProcedureStep baseProcedureStep = procedureStep;

        //    baseProcedureStep.LinkProcedure(new Procedure());
        //}

        #endregion

        private static bool RoughlyEqual(DateTime? x, DateTime? y)
        {
            if (!x.HasValue && !y.HasValue)
                return true;

            if (!x.HasValue || !y.HasValue)
                return false;

            DateTime xx = x.Value;
            DateTime yy = y.Value;

            // for these purposes, if the times are within 1 second, that is good enough
            return Math.Abs((xx - yy).TotalSeconds) < 1;
        }
    }
}

#endif