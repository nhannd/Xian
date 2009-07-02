#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Tests
{
    [TestFixture]
    public class CancelOrderOperationTests
    {
        [Test]
        public void Test_Execute()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            CancelOrderOperation op = new CancelOrderOperation();

            Assert.AreEqual(OrderStatus.SC, order.Status);

            op.Execute(order, info);
            order.UpdateStatus();

            Assert.AreEqual(OrderStatus.CA, order.Status);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_InProgress()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            CancelOrderOperation op = new CancelOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            op.Execute(order, info);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Execute_Complete()
        {
            Order order = new Order();
            OrderCancelInfo info = new OrderCancelInfo();
            CancelOrderOperation op = new CancelOrderOperation();
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
            CancelOrderOperation op = new CancelOrderOperation();
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
            CancelOrderOperation op = new CancelOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            op.Execute(order, info);
        }

        [Test]
        public void Test_CanExecute()
        {
            Order order = new Order();
            CancelOrderOperation op = new CancelOrderOperation();
            
            Assert.AreEqual(OrderStatus.SC, order.Status);
            Assert.IsTrue(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_InProgress()
        {
            Order order = new Order();
            CancelOrderOperation op = new CancelOrderOperation();
            Procedure procedure = new Procedure();
            ModalityProcedureStep step = new ModalityProcedureStep(procedure, "New modality.", new Healthcare.Modality());
            order.AddProcedure(procedure);

            step.Start(new Staff());
            Assert.AreEqual(OrderStatus.IP, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }

        [Test]
        public void Test_CanExecute_Complete()
        {
            Order order = new Order();
            CancelOrderOperation op = new CancelOrderOperation();
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
            CancelOrderOperation op = new CancelOrderOperation();
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
            CancelOrderOperation op = new CancelOrderOperation();

            order.Cancel(new OrderCancelInfo());
            order.UpdateStatus();
            Assert.AreEqual(OrderStatus.CA, order.Status);

            Assert.IsFalse(op.CanExecute(order));
        }
    }
}

#endif
