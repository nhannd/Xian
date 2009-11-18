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

namespace ClearCanvas.Healthcare.Workflow.Tests
{
    [TestFixture]
    public class DiscontinueOrderOperationTests
    {
        class ConcreteReportingProcedureStep : ReportingProcedureStep
        {

            public ConcreteReportingProcedureStep(Procedure procedure)
                : base(procedure, new ReportPart())
            {

            }

            public override string Name
            {
                get { return "Concrete Reporting Procedure Step"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [Test]
        public void Test_Execute()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Scheduling()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            Assert.AreEqual(OrderStatus.SC, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Complete()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            procedure.Complete(DateTime.Now);
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CM, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Discontinue()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            order.Discontinue(new OrderCancelInfo());
            Assert.AreEqual(OrderStatus.DC, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Cancel()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            op.Execute(order, info);
        }

        [Test]
        public void Test_CanExecute_InProgress()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            Assert.IsTrue(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            Assert.AreEqual(OrderStatus.SC, order.Status);
            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Complete()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            procedure.Complete(DateTime.Now);
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CM, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Discontinue()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            order.Discontinue(new OrderCancelInfo());
            Assert.AreEqual(OrderStatus.DC, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Cancel()
        {
            Order order = new Order();
            DiscontinueOrderOperation op = new DiscontinueOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }
    }
}

#endif
