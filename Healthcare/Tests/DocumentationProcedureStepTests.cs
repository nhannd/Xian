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