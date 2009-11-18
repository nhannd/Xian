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

using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProtocolProcedureStepTests
    {
        class ConcreteProtocolProcedureStep : ProtocolProcedureStep
        {
            public ConcreteProtocolProcedureStep(Protocol protocol)
                :base(protocol)
            {
                
            }

            public override string Name
            {
                get { return "whatever"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new ConcreteProtocolProcedureStep(this.Protocol);
            }
        }
        class GenericReportingProcedureStep : ReportingProcedureStep
        {
            public GenericReportingProcedureStep(Procedure procedure)
                : base(procedure, new ReportPart())
            {
                
            }

            public override string Name
            {
                get { return "Generic Reporting"; }
            } 

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new GenericReportingProcedureStep(this.Procedure);
            }
        }

        #region Property Tests

        [Test]
        public void Test_IsPreStep()
        {
            Protocol protocol = new Protocol();
            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);

            Assert.IsTrue(procedureStep.IsPreStep);
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);

            Assert.AreEqual(protocol, procedureStep.Protocol);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_GetLinkedProcedures_NoLinkedProcedures()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);

            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);
            procedureStep.Procedure = procedure; //TODO why isn't this passed to constructor?

            Assert.IsNotNull(procedureStep.Protocol);
            Assert.AreEqual(1, procedureStep.Protocol.Procedures.Count);
            Assert.IsNotNull(procedureStep.GetLinkedProcedures());
            Assert.IsEmpty(procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetLinkedProcedures_OneLinkedProcedure()
        {
            Procedure procedure = new Procedure();
            Procedure procedure2 = new Procedure();
            Protocol protocol = new Protocol(procedure);
            protocol.Procedures.Add(procedure2);

            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);
            procedureStep.Procedure = procedure; //TODO why isn't this passed to constructor?

            Assert.IsNotNull(procedureStep.Protocol);
            Assert.AreEqual(2, procedureStep.Protocol.Procedures.Count);
            Assert.AreEqual(1, procedureStep.GetLinkedProcedures().Count);
            Assert.Contains(procedure2, procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetRelatedProcedureSteps()
        {
            Procedure procedure = new Procedure();

            // Testing that procedure steps with tied protocols will be related steps
            Protocol protocol = new Protocol(procedure);
            ConcreteProtocolProcedureStep ps1 = new ConcreteProtocolProcedureStep(protocol);
            procedure.AddProcedureStep(ps1);
            ConcreteProtocolProcedureStep ps2 = new ConcreteProtocolProcedureStep(protocol);
            procedure.AddProcedureStep(ps2);

            // expect that each ps is tied by common protocol
            Assert.AreEqual(protocol, ps1.Protocol);
            Assert.AreEqual(protocol, ps2.Protocol);
            Assert.Contains(ps2, ps1.GetRelatedProcedureSteps());
            Assert.Contains(ps1, ps2.GetRelatedProcedureSteps());
            
            // Testing that the relative has to be a protocol step
            GenericReportingProcedureStep ps3 = new GenericReportingProcedureStep(procedure);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps3));

            // expect that the related psteps are not related to the different step
            Assert.IsFalse(ps3.GetRelatedProcedureSteps().Contains(ps1));
            Assert.IsFalse(ps3.GetRelatedProcedureSteps().Contains(ps2));
            Assert.IsFalse(ps1.GetRelatedProcedureSteps().Contains(ps3));
            Assert.IsFalse(ps2.GetRelatedProcedureSteps().Contains(ps3));
        }
 
        #endregion
    }
}

#endif