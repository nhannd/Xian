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
using ClearCanvas.Workflow;
using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class InterpretationStepTests
    {
        #region Property Test

        [Test]
        public void Test_Name()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            Assert.AreEqual("Interpretation", procedureStep.Name);
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor_Procedure()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            Assert.IsNull(procedureStep.ReportPart);
        }

        #endregion

        #region Method Tests

        // LinkProcedure in Report does not allow a null Report to be linked to another
        // one, understandably so.... 
        // TODO: This test is incomplete or not required, I'm not sure yet...
        //[Test]
        //public void Test_LinkTo()
        //{
        //    Procedure p1 = new Procedure();
        //    Report r1 = new Report(p1);
        //    Procedure p2 = new Procedure();
        //    Report r2 = new Report(p2);
            
        //    InterpretationStep ps1 = new InterpretationStep(p1),
        //                       ps2 = new InterpretationStep(p2);
        //    ps1.ReportPart = new ReportPart(r1, 0);
        //    ps2.ReportPart = new ReportPart(r2, 0);

        //    ps1.LinkTo(ps2);

        //    Assert.IsTrue(ps2.Report.Procedures.Contains(p1));
        //}

        [Test]
        [ExpectedException(typeof(WorkflowException), "This step must be associated with a Report before procedures can be linked.")]
        public void Test_LinkTo_NullReport()
        {
            InterpretationStep ps1 = new InterpretationStep(new Procedure());
            InterpretationStep ps2 = new InterpretationStep(new Procedure());
            ps1.LinkTo(ps2);
        }

        // assumption is suspend represents every other state from which InterpretationStep could
        // go to from that is not "Complete" and thereby will not trigger the exception and will
        // set the ActivityStatus properly
        [Test]
        public void Test_Suspend()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_NullReport()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            procedureStep.Complete();
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            InterpretationStep newStep = (InterpretationStep)procedureStep.Reassign(new Staff());

            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOfType(typeof (InterpretationStep), newStep);
        }
        #endregion
    }
}

#endif