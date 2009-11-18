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

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProtocolResolutionStepTests
    {
        [Test]
        public void Test_ShouldCancel()
        {
            Protocol protocol = new Protocol(new Procedure());
            ProtocolResolutionStep procedureStep = new ProtocolResolutionStep(protocol);
            procedureStep.Protocol.Reject(
                new ProtocolRejectReasonEnum(Convert.ToString(ProtocolRejectReasonEnum.DescriptionLength), "GRJ", "Generic Rejection"));

            Assert.IsTrue(procedureStep.ShouldCancel);
        }

        [Test]
        public void Test_Name()
        {
            Protocol protocol = new Protocol(new Procedure());
            ProtocolResolutionStep procedureStep = new ProtocolResolutionStep(protocol);

            Assert.AreEqual("Protocol Resolution", procedureStep.Name);
        }

        // CreateScheduledCopy() in this class is identical to the one found 
        // in ProtocolAssignmentStep, assumption is that they both function identically as well.
        // Nonetheless test is copied over and is also identical to the one found in
        // ProtocolAssignmentStepTests

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            Staff staff1 = new Staff(),
                  staff2 = new Staff();
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep(protocol);
            ps1.Procedure = procedure;
            ps1.Start(staff1);

            ProtocolAssignmentStep ps2 = (ProtocolAssignmentStep)ps1.Reassign(staff2);

            Assert.AreNotEqual(ps1, ps2);
            Assert.AreEqual(ps1.Protocol, ps2.Protocol);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps2));
        }
    }
}

#endif