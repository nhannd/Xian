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