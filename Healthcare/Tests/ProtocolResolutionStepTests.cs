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