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
            Assert.AreEqual(0, procedure.ProcedureSteps.Count);
            Assert.IsNotNull(procedure.ProcedureCheckIn);
            Assert.AreEqual(0, procedure.Reports.Count);
            Assert.AreEqual(0, procedure.Protocols.Count);
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
            procedure.Complete(Platform.Time);

            Assert.AreEqual(ProcedureStatus.CM, procedure.Status);
            Assert.IsTrue(procedure.IsTerminated);

            procedure = new Procedure();
            procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());
            procedure.Discontinue();

            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
            Assert.IsTrue(procedure.IsTerminated);
        }

        [Test]
        public void Test_IsTerminatedFalse()
        {
            Procedure procedure = new Procedure();

            Assert.IsFalse(procedure.IsTerminated);

            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());

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
            Assert.IsNull(procedure.ActiveReport);

            // add a report, which is the active report
            Report r1 = new Report(procedure);
            Assert.AreEqual(r1, procedure.ActiveReport);

            // cancel r1, so there is no acive report
            r1.Parts[0].Cancel();
            Assert.IsNull(procedure.ActiveReport);

            // add r2 which is then the active report
            Report r2 = new Report(procedure);
            Assert.AreEqual(r2, procedure.ActiveReport);
        }

        [Test]
        public void Test_ActiveProtocol()
        {
            Procedure procedure = new Procedure();
            Assert.IsNull(procedure.ActiveProtocol);

            Protocol p1 = new Protocol(procedure);
            Assert.AreEqual(p1, procedure.ActiveProtocol);

            p1.Cancel();
            Assert.IsNull(procedure.ActiveProtocol);

            Protocol p2 = new Protocol(procedure);
            Assert.AreEqual(p2, procedure.ActiveProtocol);
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
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());
            ModalityProcedureStep ps3 = new ModalityProcedureStep(procedure, "ps3", new Modality());

            Assert.IsFalse(procedure.IsPerformed);
            
            ps1.Start(new Staff());
            Assert.IsFalse(procedure.IsPerformed);
            
            ps1.Complete(Platform.Time);
            Assert.IsFalse(procedure.IsPerformed);

            ps2.Start(new Staff());
            ps3.Start(new Staff());
            Assert.IsFalse(procedure.IsPerformed);

            ps2.Discontinue();
            Assert.IsFalse(procedure.IsPerformed);

            ps3.Complete();
            Assert.IsTrue(procedure.IsPerformed);
        }

        [Test]
        public void Test_IsPerformed_NonePerformed()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
            ps1.Start(new Staff());

            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());

            Assert.IsFalse(procedure.IsPerformed);
        }

        #endregion

        #region Public Operations Tests

        [Test]
        public void Test_GetProcedureSteps()
        {
            Procedure procedure = new Procedure();

            Assert.IsEmpty(procedure.GetProcedureSteps(delegate { return true; }));

            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "1", new Modality());
            ModalityProcedureStep ps3 = new ModalityProcedureStep(procedure, "2", new Modality());
            ProtocolAssignmentStep ps4 = new ProtocolAssignmentStep();
            procedure.AddProcedureStep(ps4);


            Assert.AreEqual(4, procedure.GetProcedureSteps(delegate { return true; }).Count);
            Assert.IsEmpty(procedure.GetProcedureSteps(delegate { return false; }));
            Assert.AreEqual(1, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is DocumentationProcedureStep;
                                                               }).Count);
            Assert.AreEqual(2, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ModalityProcedureStep;
                                                               }).Count);
            Assert.AreEqual(0, procedure.GetProcedureSteps(delegate(ProcedureStep ps)
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
            Assert.IsNull(procedure.GetProcedureStep(delegate { return true; }));

            DocumentationProcedureStep ps1 = new DocumentationProcedureStep(procedure);
            ModalityProcedureStep ps2 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
            ModalityProcedureStep ps3 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
            ConcreteReportingProcedureStep ps4 = new ConcreteReportingProcedureStep(procedure);

            Assert.IsNull(procedure.GetProcedureStep(delegate { return false; }));

            Assert.AreEqual(ps1, procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is DocumentationProcedureStep;
                                                               }));
            Assert.AreEqual(ps2, procedure.GetProcedureStep(delegate(ProcedureStep ps)
                                                               {
                                                                   return ps is ModalityProcedureStep;
                                                               }));
            Assert.AreEqual(ps4, procedure.GetProcedureStep(delegate(ProcedureStep ps)
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
            Assert.IsFalse(procedure.ProcedureSteps.Contains(ps));
            Assert.AreNotEqual(procedure, ps.Procedure);

            procedure.AddProcedureStep(ps);

            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps));
            Assert.AreEqual(procedure, ps.Procedure);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_AddProcedureStep_StepProcedureNotNull()
        {
            Procedure p1 = new Procedure();
            DocumentationProcedureStep ps = new DocumentationProcedureStep();
            Assert.IsNull(ps.Procedure);

            p1.AddProcedureStep(ps);
            Assert.AreEqual(p1, ps.Procedure);

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

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);

            procedure.AddProcedureStep(procedureStep);
        }

        [Test]
        public void Test_Schedule()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep ps = new DocumentationProcedureStep(procedure);
            DateTime? now = DateTime.Now;
            DateTime? later = now + TimeSpan.FromDays(3);

            Assert.IsNull(ps.Scheduling.StartTime);
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
            Assert.AreEqual(ActivityStatus.SC, ps.State);

            procedure.Schedule(now);

            Assert.AreEqual(now, ps.Scheduling.StartTime);
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
            Assert.AreEqual(ActivityStatus.SC, ps.State);

            procedure.Schedule(later);

            Assert.AreEqual(later, ps.Scheduling.StartTime);
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
            Assert.AreEqual(ActivityStatus.SC, ps.State);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_Cancelled()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();
            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

            procedure.Schedule(Platform.Time);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_Discontinued()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Discontinue();
            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

            procedure.Schedule(Platform.Time);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_InProgress()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            Assert.AreEqual(ProcedureStatus.IP, procedure.Status);

            procedure.Schedule(Platform.Time);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Schedule_Complete()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Complete(DateTime.Now);
            Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

            procedure.Schedule(Platform.Time);
        }

        [Test]
        public void Discontinue()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
            procedureStep.Start(new Staff());

            Assert.AreEqual(ProcedureStatus.IP, procedure.Status);
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);

            procedure.Discontinue();

            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
            Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Discontinue_Cancelled()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();

            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

            procedure.Discontinue();
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Discontinue_Scheduling()
        {
            Procedure procedure = new Procedure();
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);

            procedure.Discontinue();
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Discontinue_Complete()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Complete(DateTime.Now);
            Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

            procedure.Discontinue();
        }

        [Test]
        [ExpectedException((typeof(WorkflowException)))]
        public void Test_Discontinue_DiscontinuedState()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Discontinue();
            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

            procedure.Discontinue();
        }

        [Test]
        public void Test_Cancel()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep ps = new ConcreteProcedureStep(procedure);
            Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
            Assert.AreEqual(ActivityStatus.SC, ps.State);

            procedure.Cancel();

            Assert.AreEqual(ActivityStatus.DC, ps.State);
            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);
        }
        
        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Cancel_CancelledState()
        {
            Procedure procedure = new Procedure();
            procedure.Cancel();
            Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

            procedure.Cancel();
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Cancel_InProgress()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            Assert.AreEqual(ProcedureStatus.IP, procedure.Status);

            procedure.Cancel();
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Cancel_Discontinued()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Discontinue();
            Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

            procedure.Cancel();
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Cancel_Completed()
        {
            Procedure procedure = new Procedure();
            ModalityProcedureStep ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
            ps.Start(new Staff());
            procedure.Complete(DateTime.Now);
            Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

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

        /// NOTE: STRUCTURE OF TEST WORKFLOW HISTORY "TREES"
        // Procedure <-> ProcedureStep relationship:
        // -> In keeping with the model, 1 Procedure has multiple ProcedureSteps
        // -> Every two sibling nodes/children on each level are each constructed
        //    with ONE procedure
        // ProcedureStep <-> ProcedureStep relationship:
        // -> ProcedureSteps of the same parent node (except for the first level) are
        //    "IsRelatedStep" to each other
        // -> ProcedureSteps of adjacent levels are related via Linking, and the "IsLinked"
        //    property.
        // -> Only the first (or leftmost) child node of the adjacent level is linked,
        //    ProcedureSteps within the same level are related as previously mentioned.
        // The following ASCII-drawn diagram shows a rudimentary illustration of how a basic
        // binary tree-like test workflow history is constructed.
        // 
        // NOTE: Diagram nomenclature
        // -> px, where x is a number, letter, or word: Procedure
        // -> psx, where x is a number, letter, or word: ProcedureStep
        // -> px: all ProcedureSteps at this level are constructed with this Procedure
        // -> |
        //    |---->, directory style arrow indicates ProcedureStep linking
        // -> "->psx", indicates another ProcedureStep on the same level as the one above it

        ///  pRoot:
        ///  ->ps1
        ///  |   p1:
        ///  |---->ps11
        ///      |   p11:
        ///      |---->ps111
        ///          ->ps112
        ///      ->ps12
        ///      |   p12:
        ///      |---->ps121
        ///          ->ps122
        ///  ->ps2
        ///  |   p2:
        ///  |---->ps21
        ///      |   p21:
        ///      |---->ps211
        ///          ->ps212
        ///      ->ps22
        ///      |   p22:
        ///      |---->ps221
        ///          ->ps222
            

        [Test]
        public void Test_GetWorkflowHistory_TwoLevel()
        {
            // Testing an ideal 6 item Workflow history "tree"

            Procedure pRoot = new Procedure(); // associative Procedure to first level
            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot); // First level

            Procedure p1 = new Procedure(); // associative Procedure to left branch of second level
            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1); // Second level, left branch
            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1); // Second level, left branch
            ps11.AddRelatedStep(ps12); // Relate children
            ps1.LinkTo(ps11); // Link adjacent levels

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot); // First level

            Procedure p2 = new Procedure(); // associative Procedure to right branch of second level
            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2); // Second level, right branch
            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2); // Second level, right branch
            ps21.AddRelatedStep(ps22); // Relate children
            ps2.LinkTo(ps21); // Link adjacent levels

            Assert.AreEqual(6, pRoot.GetWorkflowHistory().Count); // Assert all items in "tree" are present in Workflow history
        }

        [Test]
        public void Test_GetWorkflowHistory_ThreeLevel()
        {
            Procedure pRoot = new Procedure(); // associative Procedure to first level
            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot);

            Procedure p1 = new Procedure(); // associative Procedure to left branch of second level
            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1);

            Procedure p11 = new Procedure(); // associative Procedure to leftmost branch of third level
            ConcreteProcedureStep ps111 = new ConcreteProcedureStep(p11);
            ConcreteProcedureStep ps112 = new ConcreteProcedureStep(p11);
            ps111.AddRelatedStep(ps112); // relate steps of p11
            ps11.LinkTo(ps111); // link adjacent levels

            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1);

            Procedure p12 = new Procedure(); // associative Procedure to second leftmost branch of third level
            ConcreteProcedureStep ps121 = new ConcreteProcedureStep(p12);
            ConcreteProcedureStep ps122 = new ConcreteProcedureStep(p12);
            ps121.AddRelatedStep(ps122); // relate steps of p12
            ps12.LinkTo(ps121); // link adjacent levels
            ps11.AddRelatedStep(ps12); // relate steps of p1

            ps1.LinkTo(ps11); // link adjacent levels

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot);

            Procedure p2 = new Procedure(); // associative Procedure to right branch of second level
            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2);

            Procedure p21 = new Procedure(); // associative Procedure to second rightmost branch of third level
            ConcreteProcedureStep ps211 = new ConcreteProcedureStep(p21);
            ConcreteProcedureStep ps212 = new ConcreteProcedureStep(p21);
            ps211.AddRelatedStep(ps212); // relate steps of p21
            ps21.LinkTo(ps211); // link adjacent levels

            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2);

            Procedure p22 = new Procedure(); // associative Procedure to rightmost branch of third level
            ConcreteProcedureStep ps221 = new ConcreteProcedureStep(p22);
            ConcreteProcedureStep ps222 = new ConcreteProcedureStep(p22);
            ps221.AddRelatedStep(ps222); // relate steps of p22
            ps22.LinkTo(ps221); // link adjacent levels
            ps21.AddRelatedStep(ps22); // relate steps of p2

            ps2.LinkTo(ps21); // link adjacent levels

            Assert.AreEqual(14, pRoot.GetWorkflowHistory().Count); // Assert all items in "tree" are present in Workflow history
        }

        [Test]
        public void Test_GetWorkflowHistory_UnrelatedSteps()
        {
            // This "tree" doesn't have the right children of each pair in the third level
            // Due to the abscence of relating these steps

            Procedure pRoot = new Procedure(); // associative Procedure to first level
            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(pRoot);

            Procedure p1 = new Procedure(); // associative Procedure to left branch of second level
            ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1);

            Procedure p11 = new Procedure(); // associative Procedure to leftmost branch of third level
            ConcreteProcedureStep ps111 = new ConcreteProcedureStep(p11);
            ConcreteProcedureStep ps112 = new ConcreteProcedureStep(p11); // is not related to ps111
            ps11.LinkTo(ps111); // link adjacent levels

            ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1);

            Procedure p12 = new Procedure(); // associative Procedure to second leftmost branch of third level
            ConcreteProcedureStep ps121 = new ConcreteProcedureStep(p12);
            ConcreteProcedureStep ps122 = new ConcreteProcedureStep(p12); // is not related to ps121
            ps12.LinkTo(ps121); // link adjacent levels
            ps11.AddRelatedStep(ps12); // relate steps of p1

            ps1.LinkTo(ps11); // link adjacent levels

            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(pRoot);

            Procedure p2 = new Procedure(); // associative Procedure to right branch of second level
            ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2);

            Procedure p21 = new Procedure(); // associative Procedure to second rightmost branch of third level
            ConcreteProcedureStep ps211 = new ConcreteProcedureStep(p21);
            ConcreteProcedureStep ps212 = new ConcreteProcedureStep(p21); // is not related to ps211
            ps21.LinkTo(ps211); // link adjacent levels

            ConcreteProcedureStep ps22 = new ConcreteProcedureStep(p2);

            Procedure p22 = new Procedure(); // associative Procedure to rightmost branch of third level
            ConcreteProcedureStep ps221 = new ConcreteProcedureStep(p22);
            ConcreteProcedureStep ps222 = new ConcreteProcedureStep(p22); // is not related to ps221
            ps22.LinkTo(ps221); // link adjacent levels
            ps21.AddRelatedStep(ps22); // relate steps of p2

            ps2.LinkTo(ps21); // link adjacent levels

            Assert.AreEqual(10, pRoot.GetWorkflowHistory().Count); 
            // Assert all items in "tree" are present in Workflow history... except these ones:
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
