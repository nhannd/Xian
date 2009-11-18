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
using ClearCanvas.Workflow;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class DocumentationProcedureStepTests
    {
        #region Property Tests

        [Test]
        public void Test_Name()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep procedureStep = new DocumentationProcedureStep(procedure);

            Assert.AreEqual("Documentation", procedureStep.Name);
        }

        [Test]
        public void Test_IsPreStep()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep procedureStep = new DocumentationProcedureStep(procedure);

            Assert.IsFalse(procedureStep.IsPreStep);
        }

        #endregion

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep procedureStep = new DocumentationProcedureStep(procedure);

            Assert.AreEqual(procedure, procedureStep.Procedure);
        }

        [Test]
        public void Test_GetLinkedProcedures()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep procedureStep = new DocumentationProcedureStep(procedure);

            Assert.IsNotNull(procedureStep.GetLinkedProcedures());
            Assert.IsEmpty(procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            DocumentationProcedureStep procedureStep = new DocumentationProcedureStep(procedure);
            procedureStep.Suspend();

            Staff performer = new Staff();
            ProcedureStep newStep = procedureStep.Reassign(performer); // Perform event

            // just need to test that it returns a new instance of this class
            // everything else has been covered in base class tests
            Assert.IsNotNull(newStep);
            Assert.AreNotEqual(this, newStep);
            Assert.IsInstanceOfType(typeof(DocumentationProcedureStep), newStep);
            Assert.AreEqual(procedure, newStep.Procedure);
        }

        [Test]
        public void Test_GetRelatedProcedureSteps()
        {
            Procedure p1 = new Procedure();

            // attach 2 procedure steps to p1
            DocumentationProcedureStep ps11 = new DocumentationProcedureStep(p1);
            DocumentationProcedureStep ps12 = new DocumentationProcedureStep(p1);

            // expect that each ps is only related to itself
            Assert.IsEmpty(ps11.GetRelatedProcedureSteps());
            Assert.IsEmpty(ps12.GetRelatedProcedureSteps());
        }
    }
}

#endif