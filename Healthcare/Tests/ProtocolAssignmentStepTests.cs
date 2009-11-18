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
    public class ProtocolAssignmentStepTests
    {
        #region Property Tests

        [Test]
        public void Test_CanAcceptRejctSuspendSave()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(new Staff());

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(procedureStep.CanAccept);
            Assert.IsTrue(procedureStep.CanReject);
            Assert.IsTrue(procedureStep.CanSuspend);
            Assert.IsTrue(procedureStep.CanSave);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
            Assert.IsFalse(procedureStep.CanAccept);
            Assert.IsFalse(procedureStep.CanReject);
            Assert.IsFalse(procedureStep.CanSuspend);
            Assert.IsFalse(procedureStep.CanSave);

            procedureStep.Complete();

            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.IsFalse(procedureStep.CanAccept);
            Assert.IsFalse(procedureStep.CanReject);
            Assert.IsFalse(procedureStep.CanSuspend);
            Assert.IsFalse(procedureStep.CanSave);
        }

        [Test]
        public void Test_CanApprove()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(protocol);

            Assert.AreEqual(ProtocolStatus.PN, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            protocol.SubmitForApproval();

            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsTrue(procedureStep.CanApprove);

            procedureStep.Start(new Staff());

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsTrue(procedureStep.CanApprove);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            procedureStep.Complete();

            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            // TODO : test all other Protocol state conditions
        }

        [Test]
        public void Test_CanEdit()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());
            Staff staff = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);
            Assert.IsFalse(procedureStep.CanEdit(staff));

            procedureStep.Start(staff);

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(staff, procedureStep.PerformingStaff);
            Assert.IsTrue(procedureStep.CanEdit(staff));
            Assert.IsFalse(procedureStep.CanEdit(new Staff()));

            // TODO : test all other Activity state conditions
        }

        [Test]
        public void Test_Name()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());

            Assert.AreEqual("Protocol Assignment", procedureStep.Name);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_LinkTo()
        {
            Procedure p1 = new Procedure();
            Procedure p2 = new Procedure();
            Protocol protocol = new Protocol(p1);
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep(protocol),
                                   ps2 = new ProtocolAssignmentStep(new Protocol(p2));
            ps1.Procedure = p1;
            ps2.Procedure = p2;
            ps1.LinkTo(ps2);

            Assert.IsTrue(ps2.Protocol.Procedures.Contains(p1));
            Assert.Contains(p1, ps2.GetLinkedProcedures());
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_LinkTo_NullProtocol()
        {
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep();
            ProtocolAssignmentStep ps2 = new ProtocolAssignmentStep();
            ps1.LinkTo(ps2);
        }

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

            ProcedureStep ps2 = ps1.Reassign(staff2);

            Assert.IsInstanceOfType(typeof (ProtocolAssignmentStep), ps2);
            Assert.AreNotEqual(ps1, ps2);
            Assert.AreEqual(ps1.Protocol, ((ProtocolAssignmentStep)ps2).Protocol);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps2));
        }

        #endregion
    }
}

#endif