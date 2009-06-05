#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class VerificationStepTests
    {
        [Test]
        public void Test_Name()
        {
            VerificationStep procedureStep = new VerificationStep();

            Assert.AreEqual("Verification", procedureStep.Name);
        }

        [Test]
        public void Test_Complete()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            InterpretationStep previousStep = new InterpretationStep(procedure);
            previousStep.ReportPart = reportPart;
            VerificationStep procedureStep = new VerificationStep(previousStep);
            procedureStep.Start(performer);

            procedureStep.Complete();

            Assert.AreEqual(performer, procedureStep.ReportPart.Verifier);
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            reportPart.Supervisor = new Staff();
            VerificationStep procedureStep = new VerificationStep();
            procedureStep.Procedure = procedure;
            procedureStep.ReportPart = reportPart;
            Assert.IsNotNull(procedureStep.ReportPart);

            VerificationStep newStep = (VerificationStep)procedureStep.Reassign(performer);

            Assert.AreEqual(performer, newStep.ReportPart.Supervisor);
            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOfType(typeof(VerificationStep), newStep);
        }
    }
}

#endif