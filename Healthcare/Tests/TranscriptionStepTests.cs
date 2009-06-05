#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class TranscriptionStepTests
    {
        [Test]
        public void Test_Name()
        {
            TranscriptionStep procedureStep = new TranscriptionStep();

            Assert.AreEqual("Transcription", procedureStep.Name);
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
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);
            procedureStep.Start(performer);
            
            procedureStep.Complete();

            Assert.AreEqual(performer, procedureStep.ReportPart.Transcriber);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_NullReportPart()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            InterpretationStep previousStep = new InterpretationStep(procedure);
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);
            procedureStep.Start(performer);
            
            procedureStep.Complete();
        }

        [Test]
        public void Test_Reassign()
        {
            InterpretationStep previousStep = new InterpretationStep(new Procedure());
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);

            TranscriptionStep newStep = (TranscriptionStep)procedureStep.Reassign(new Staff());

            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOfType(typeof(TranscriptionStep), newStep);
        }
    }
}

#endif