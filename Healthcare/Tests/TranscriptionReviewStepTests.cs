#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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