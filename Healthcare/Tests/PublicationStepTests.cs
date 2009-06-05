#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class PublicationStepTests
    {
        #region Property Tests

        [Test]
        public void Test_Name()
        {
            PublicationStep procedureStep = new PublicationStep();

            Assert.AreEqual("Publication", procedureStep.Name);
        }

        [Test]
        public void Test_FailureCount()
        {
            PublicationStep procedureStep = new PublicationStep();

            Assert.AreEqual(0, procedureStep.FailureCount);
        }

        [Test]
        public void Test_LastFailureTime()
        {
            PublicationStep procedureStep = new PublicationStep();

            Assert.IsNull(procedureStep.LastFailureTime);
        }

        #endregion
        
        #region Constructor Test

        [Test]
        public void Test_Constructor_PreviousStep()
        {
            InterpretationStep previousStep = new InterpretationStep(new Procedure());
            PublicationStep newStep = new PublicationStep(previousStep);

            Assert.AreEqual(0, newStep.FailureCount);
            Assert.IsNull(newStep.LastFailureTime);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_Fail()
        {
            PublicationStep procedureStep = new PublicationStep();

            procedureStep.Fail();

            Assert.AreEqual(1, procedureStep.FailureCount);
            Assert.IsTrue(RoughlyEqual(procedureStep.LastFailureTime, Platform.Time));
        }

        [Test]
        public void Test_Complete()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            InterpretationStep previousStep = new InterpretationStep(procedure);
            previousStep.ReportPart = reportPart;
            PublicationStep procedureStep = new PublicationStep(previousStep);
            procedureStep.Start(new Staff());
            procedure.UpdateStatus();

            procedureStep.Complete();

            Assert.AreEqual(ReportStatus.F, procedureStep.ReportPart.Status);
            Assert.AreEqual(0, procedureStep.ReportPart.Index);
            Assert.IsTrue(procedureStep.AllProcedures.TrueForAll(
                delegate(Procedure p)
                {
                    return p.Status == ProcedureStatus.CM;
                }));
            Assert.IsTrue(procedureStep.AllProcedures.TrueForAll(
                delegate(Procedure p)
                {
                    return p.EndTime == procedureStep.EndTime;
                }));
        }

        [Test]
        public void Test_Reassign()
        {
            InterpretationStep previousStep = new InterpretationStep(new Procedure());
            PublicationStep procedureStep = new PublicationStep(previousStep);

            PublicationStep newStep = (PublicationStep)procedureStep.Reassign(new Staff());

            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOfType(typeof(PublicationStep), newStep);
        }

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