#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class TranscriptionReviewStepTests
    {
        [Test]
        public void Test_Name()
        {
            TranscriptionReviewStep reviewStep = new TranscriptionReviewStep();

            Assert.AreEqual("Transcription Review", reviewStep.Name);
        }

        [Test]
        public void Test_Reassign()
        {
            InterpretationStep previousStep = new InterpretationStep(new Procedure());
            TranscriptionReviewStep reviewStep = new TranscriptionReviewStep(previousStep);

            TranscriptionReviewStep newStep = (TranscriptionReviewStep)reviewStep.Reassign(new Staff());

            Assert.AreEqual(reviewStep, newStep);
            Assert.IsInstanceOfType(typeof(TranscriptionReviewStep), newStep);
        }

    }
}

#endif