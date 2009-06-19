#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using Iesi.Collections.Generic;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProcedureTests
    {
        class ConcreteProcedureStep : ProcedureStep
        {
            private List<Procedure> _linkedProcedures = new List<Procedure>();
            private List<ProcedureStep> _relatedSteps = new List<ProcedureStep>();

            public ConcreteProcedureStep(Procedure procedure)
                : base(procedure)
            {
                _relatedSteps.Add(this);
            }

            public void AddRelatedStep(ProcedureStep step)
            {
                if(!Equals(step.Procedure, this.Procedure))
                    throw new Exception();

                _relatedSteps.Add(step);
            }

            protected override void LinkProcedure(Procedure procedure)
            {
                _linkedProcedures.Add(procedure);
            }

            public override string Name
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public override bool IsPreStep
            {
                get { return true; }
            }

            public override List<Procedure> GetLinkedProcedures()
            {
                return _linkedProcedures;
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            protected override bool IsRelatedStep(ProcedureStep step)
            {
                return _relatedSteps.Contains(step);
            }
        }
        class ConcreteReportingProcedureStep : ReportingProcedureStep
        {

            public ConcreteReportingProcedureStep(Procedure procedure)
                :base(procedure, new ReportPart())
            {
                
            }

            public override string Name
            {
                get { return "Concrete Reporting Procedure Step"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #region Constructor Tests

        [Test]
        public void Test_Constructor()
        {
            ProcedureType procedureType = new ProcedureType();
            Procedure procedure = new Procedure(procedureType);

            Assert.AreEqual(procedureType, procedure.Type);
            Assert.IsInstanceOfType(typeof (HashedSet<ProcedureStep>), procedure.ProcedureSteps);
            Assert.IsInstanceOfType(typeof(ProcedureCheckIn), procedure.ProcedureCheckIn);
            Assert.IsInstanceOfType(typeof(HashedSet<Report>), procedure.Reports);
            Assert.IsInstanceOfType(typeof(HashedSet<Protocol>), procedure.Protocols);
        }

        #endregion

        #region Property Tests

        [Test]
        public void Test_ModailtyProcedureSteps()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "description", new Modality());
            DocumentationProcedureStep ps2 = new DocumentationProcedureStep(procedure);

            Assert.AreEqual(2, procedure.ProcedureSteps.Count);
            Assert.IsTrue(procedure.ModalityProcedureSteps.Contains(ps1));
            Assert.AreEqual(1, procedure.ModalityProcedureSteps.Count);
        }

        [Test]
        public void Test_ReportingProcedureSteps()
        {
            Procedure procedure = new Procedure();
            ConcreteReportingProcedureStep ps1 = new ConcreteReportingProcedureStep(procedure);
            DocumentationProcedureStep ps2 = new DocumentationProcedureStep(procedure);

            Assert.AreEqual(2, procedure.ProcedureSteps.Count);
            Assert.IsTrue(procedure.ReportingProcedureSteps.Contains(ps1));
            Assert.AreEqual(1, procedure.ReportingProcedureSteps.Count);
        }

        [Test]
        public void Test_IsTerminatedTrue()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();

            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);
            Assert.IsTrue(procedure.IsTerminated);

            procedure = new Procedure();
            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());
            procedure.UpdateStatus();
            procedure.Complete(Platform.Time);

            Assert.AreEqual(ProcedureStatus.CM, procedure.Status);
            Assert.IsTrue(procedure.IsTerminated);

            procedure = new Procedure();
            procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());
            procedure.UpdateStatus();
            procedure.Discontinue();

            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
            Assert.IsTrue(procedure.IsTerminated);
        }

        [Test]
        public void Test_IsTerminatedFalse()
        {
            Procedure procedure = new Procedure();

            Assert.IsFalse(procedure.IsTerminated);
        }

        [Test]
        public void Test_DocumentationProcedureStep()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            ConcreteReportingProcedureStep ps2 = new ConcreteReportingProcedureStep(procedure);

            Assert.AreEqual(2, procedure.ProcedureSteps.Count);
            Assert.AreEqual(ps1, procedure.DocumentationProcedureStep);
        }

        [Test]
        public void Test_DocumentationProcedureStep_Null()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(procedure);
            ConcreteReportingProcedureStep ps2 = new ConcreteReportingProcedureStep(procedure);

            Assert.AreEqual(2, procedure.ProcedureSteps.Count);
            Assert.IsNull(procedure.DocumentationProcedureStep);
        }

        [Test]
        public void Test_ActiveReport()
        {
            Procedure procedure = new Procedure();
            Report r1 = new Report(procedure);
            ReportPart rp1 = new ReportPart(r1, 0);
            r1.Parts[0] = rp1;

            Report r2 = new Report(procedure);
            ReportPart rp2 = new ReportPart(r2, 0);
            r2.Parts[0] = rp2;
            rp2.Cancel();

            Assert.AreEqual(r1, procedure.ActiveReport);

            r2 = new Report(procedure);
            rp2 = new ReportPart(r2, 0);
            r2.Parts[0] = rp2;

            Assert.AreEqual(r1, procedure.ActiveReport);
        }

        [Test]
        public void Test_ActiveReport_ReturnsNull()
        {
            Procedure procedure = new Procedure();
            Report r1 = new Report(procedure);
            r1.Parts[0].Cancel();
            r1.UpdateStatus();

            Report r2 = new Report(procedure);
            r2.Parts[0].Cancel();
            r2.UpdateStatus();

            Assert.IsNull(procedure.ActiveReport);
        }

        [Test]
        public void Test_ActiveProtocol()
        {
            Procedure procedure = new Procedure();
            Protocol p1 = new Protocol(procedure);
            Protocol p2 = new Protocol(procedure);
            
            p2.Cancel();

            Assert.AreEqual(p1, procedure.ActiveProtocol);

            p2 = new Protocol(procedure);

            Assert.AreEqual(p1, procedure.ActiveProtocol);
        }

        [Test]
        public void Test_ActiveProtocol_ReturnsNull()
        {
            Procedure procedure = new Procedure();
            Protocol p1 = new Protocol(procedure);
            Protocol p2 = new Protocol(procedure);

            p1.Cancel();
            p2.Cancel();

            Assert.IsNull(procedure.ActiveProtocol);
        }

        [Test]
        public void Test_PerformedTime()
        {
            Procedure procedure = new Procedure();

            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
            ps1.Start(new Staff());
            ps1.Complete(Platform.Time);

            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());
            ps2.Start(new Staff());
            ps2.Complete(Platform.Time + TimeSpan.FromDays(1));

            ModalityProcedureStep ps3 = new ModalityProcedureStep(procedure, "ps3", new Modality());
            ps3.Start(new Staff());
            ps3.Complete(Platform.Time + TimeSpan.FromDays(2));

            ModalityProcedureStep ps4 = new ModalityProcedureStep(procedure, "ps4", new Modality());
            ps4.Start(new Staff());
            ps4.Complete(Platform.Time + TimeSpan.FromDays(3));
            
            Assert.AreEqual(ps4.EndTime, procedure.PerformedTime);
        }

        [Test]
        public void Test_PerformedTime_NoneCompleted()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());

            Assert.IsNull(procedure.PerformedTime);
        }

        [Test]
        public void Test_IsPerformed()
        {
            Procedure procedure = new Procedure();

            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
            ps1.Start(new Staff());
            ps1.Complete(Platform.Time);

            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());
            ps2.Start(new Staff());
            ps2.Complete(Platform.Time + TimeSpan.FromDays(1));

            ModalityProcedureStep ps3 = new ModalityProcedureStep(procedure, "ps3", new Modality());
            ps3.Start(new Staff());
            ps3.Complete(Platform.Time + TimeSpan.FromDays(2));

            ModalityProcedureStep ps4 = new ModalityProcedureStep(procedure, "ps4", new Modality());
            ps4.Start(new Staff());
            ps4.Complete(Platform.Time + TimeSpan.FromDays(3));

            Assert.IsTrue(procedure.IsPerformed);
        }

        [Test]
        public void Test_IsPerformed_NonePerformed()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());

            Assert.IsFalse(procedure.IsPerformed);
        }

        #endregion

        #region Public Operations Tests

        [Test]
        public void Test_GetProcedureSteps()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
            ConcreteReportingProcedureStep ps3 = new ConcreteReportingProcedureStep(procedure);
            ProtocolAssignmentStep ps4 = new ProtocolAssignmentStep();
            procedure.AddProcedureStep(ps4);

            Assert.AreEqual(1, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is DocumentationProcedureStep;
                                                               }).Count);
            Assert.AreEqual(1, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ModalityProcedureStep;
                                                               }).Count);
            Assert.AreEqual(1, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ReportingProcedureStep;
                                                               }).Count);
            Assert.AreEqual(1, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ProtocolProcedureStep;
                                                               }).Count);
        }
        
        [Test]
        public void Test_GetProcedureStep()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
            ConcreteReportingProcedureStep ps3 = new ConcreteReportingProcedureStep(procedure);

            Assert.AreEqual(ps1, procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is DocumentationProcedureStep;
                                                               }));
            Assert.AreEqual(ps2, procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ModalityProcedureStep;
                                                               }));
            Assert.AreEqual(ps3, procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ReportingProcedureStep;
                                                               }));
            Assert.IsNull(procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ProtocolProcedureStep;
                                                               }));
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_CreateProcedureSteps()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps = new DocumentationProcedureStep(procedure);

            procedure.CreateProcedureSteps();
        }

        [Test]
        public void Test_AddProcedureStep()
        {
            Procedure procedure = new Procedure();

            DocumentationProcedureStep ps = new DocumentationProcedureStep();

            procedure.AddProcedureStep(ps);

            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_AddProcedureStep_StepProcedureNotNull()
        {
            Procedure p1 = new Procedure();
            DocumentationProcedureStep ps = new DocumentationProcedureStep();
            p1.AddProcedureStep(ps);

            Procedure p2 = new Procedure();
            p2.AddProcedureStep(ps);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_AddProcedureStep_StepNotScheduling()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());
            procedure.UpdateStatus();
            procedure.Discontinue();
            procedure.AddProcedureStep(procedureStep);
        }

        [Test]
        public void Test_Schedule()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps = new DocumentationProcedureStep(procedure);
            DateTime? now = DateTime.Now;

            procedure.Schedule(now);

            Assert.AreEqual(now, ps.Scheduling.StartTime);
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
            Assert.AreEqual(ActivityStatus.SC, ps.State);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_NotScheduling()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();
            procedure.Schedule(Platform.Time);
        }

        [Test]
        public void Discontinue()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());
            procedure.UpdateStatus();
            procedure.Discontinue();

            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
            Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_NotInProgress()
        {
            Procedure procedure = new Procedure();
            procedure.Discontinue();
            procedure.Schedule(Platform.Time);
        }

        [Test]
        public void Test_Cancel()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();

            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);
        }
        
        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Cancel_NotScheduling()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();
            procedure.Cancel();
        }

        [Test]
        public void Test_GetWorkflowHistory_OneLevel()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            DocumentationProcedureStep ps2 = new DocumentationProcedureStep(procedure);

            Assert.AreEqual(2, procedure.GetWorkflowHistory().Count);
        }

        [Test]
        public void Test_GetWorkflowHistory_TwoLevel()
        {
            Procedure pRoot = new Procedure();
            Procedure p1 = new Procedure();
            Procedure p2 = new Procedure();

            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1);
            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1);
            ps11.AddRelatedStep(ps12);
            ps1.LinkTo(ps11);

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2);
            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2);
            ps21.AddRelatedStep(ps22);
            ps2.LinkTo(ps21);

            Assert.AreEqual(6, pRoot.GetWorkflowHistory().Count);
        }

        [Test]
        public void Test_GetWorkflowHistory_ThreeLevel()
        {
            Procedure pRoot = new Procedure();
            Procedure p1 = new Procedure();
            Procedure p11 = new Procedure();
            Procedure p12 = new Procedure();
            Procedure p2 = new Procedure();
            Procedure p21 = new Procedure();
            Procedure p22 = new Procedure();

            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1);

            ConcreteProcedureStep ps111 = new ConcreteProcedureStep(p11);
            ConcreteProcedureStep ps112 = new ConcreteProcedureStep(p11);
            ps111.AddRelatedStep(ps112);
            ps11.LinkTo(ps111);

            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1);

            ConcreteProcedureStep ps121 = new ConcreteProcedureStep(p12);
            ConcreteProcedureStep ps122 = new ConcreteProcedureStep(p12);
            ps121.AddRelatedStep(ps122);
            ps12.LinkTo(ps121);
            ps11.AddRelatedStep(ps12);

            ps1.LinkTo(ps11);

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2);

            ConcreteProcedureStep ps211 = new ConcreteProcedureStep(p21);
            ConcreteProcedureStep ps212 = new ConcreteProcedureStep(p21);
            ps211.AddRelatedStep(ps212);
            ps21.LinkTo(ps211);

            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2);

            ConcreteProcedureStep ps221 = new ConcreteProcedureStep(p22);
            ConcreteProcedureStep ps222 = new ConcreteProcedureStep(p22);
            ps221.AddRelatedStep(ps222);
            ps22.LinkTo(ps221);
            ps21.AddRelatedStep(ps22);

            ps2.LinkTo(ps21);

            Assert.AreEqual(14, pRoot.GetWorkflowHistory().Count);
        }

        [Test]
        public void Test_GetWorkflowHistory_UnrelatedSteps()
        {
            Procedure pRoot = new Procedure();
            Procedure p1 = new Procedure();
            Procedure p11 = new Procedure();
            Procedure p12 = new Procedure();
            Procedure p2 = new Procedure();
            Procedure p21 = new Procedure();
            Procedure p22 = new Procedure();

            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1);

            ConcreteProcedureStep ps111 = new ConcreteProcedureStep(p11);
            ConcreteProcedureStep ps112 = new ConcreteProcedureStep(p11);
            ps11.LinkTo(ps111);

            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1);

            ConcreteProcedureStep ps121 = new ConcreteProcedureStep(p12);
            ConcreteProcedureStep ps122 = new ConcreteProcedureStep(p12);
            ps12.LinkTo(ps121);
            ps11.AddRelatedStep(ps12);

            ps1.LinkTo(ps11);

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot);

            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2);

            ConcreteProcedureStep ps211 = new ConcreteProcedureStep(p21);
            ConcreteProcedureStep ps212 = new ConcreteProcedureStep(p21);
            ps21.LinkTo(ps211);

            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2);

            ConcreteProcedureStep ps221 = new ConcreteProcedureStep(p22);
            ConcreteProcedureStep ps222 = new ConcreteProcedureStep(p22);
            ps22.LinkTo(ps221);
            ps21.AddRelatedStep(ps22);

            ps2.LinkTo(ps21);

            Assert.AreEqual(10, pRoot.GetWorkflowHistory().Count);
            Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps112));
            Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps122));
            Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps212));
            Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps222));
        }

        [Test]
        public void Test_GetWorkflowHistory_Empty()
        {
            Procedure procedure = new Procedure();

            Assert.IsEmpty(procedure.GetWorkflowHistory());
        }

        #endregion
    }
}

#endif
