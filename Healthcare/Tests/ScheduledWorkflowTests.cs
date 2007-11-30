#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using System.Collections;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ScheduledWorkflowTests
    {
        private OrderCancelReasonEnum _defaultCancelReason;

        public ScheduledWorkflowTests()
        {
            _defaultCancelReason = new OrderCancelReasonEnum("x", "x", "x");

            // set the extension factory to null so we don't try to instantiate extensions from plugins
            Platform.SetExtensionFactory(new NullExtensionFactory());
        }

        /// <summary>
        /// Verify that when a new order is created, all properties are set correctly, and the diagnostic
        /// service plan is correctly applied to the order.
        /// </summary>
        [Test]
        public void CreateNewOrderScheduled()
        {
            DateTime scheduleTime = DateTime.Now;

            Patient patient = TestPatientFactory.CreatePatient();
            Visit visit = TestVisitFactory.CreateVisit(patient);
            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService();
            string accession = "10000001";
            string reasonForStudy = "Test";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();
            Facility facility = TestFacilityFactory.CreateFacility();
            IList<OrderAttachment> attachments = TestOrderAttachmentFactory.CreateOrderAttachments();

            Order order = Order.NewOrder(
                accession, patient, visit, ds, reasonForStudy, OrderPriority.R, facility, facility,
                scheduleTime, scheduleTime, orderingPrac, new List<ExternalPractitioner>(), attachments);

            // check basics
            Assert.AreEqual(accession, order.AccessionNumber);
            Assert.AreEqual(reasonForStudy, order.ReasonForStudy);
            Assert.AreEqual(patient, order.Patient);
            Assert.AreEqual(visit, order.Visit);
            Assert.AreEqual(ds, order.DiagnosticService);
            Assert.AreEqual(scheduleTime, order.SchedulingRequestDateTime);
            Assert.AreEqual(scheduleTime, order.ScheduledStartTime);    // because the order was scheduled
            Assert.AreEqual(orderingPrac, order.OrderingPractitioner);
            Assert.AreEqual(facility, order.OrderingFacility);
            Assert.AreEqual(OrderPriority.R, order.Priority);
            CheckStatus(OrderStatus.SC, order);

            // check that diagnostic service plan was copied properly
            Assert.AreEqual(ds.RequestedProcedureTypes.Count, order.RequestedProcedures.Count);
            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                CheckStatus(RequestedProcedureStatus.SC, rp);

                RequestedProcedureType rpType = CollectionUtils.SelectFirst(ds.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpt) { return rpt.Equals(rp.Type); });

                Assert.IsNotNull(rpType, "diagnostic service plan not copied correctly");
                foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
                {
                    CheckStatus(ActivityStatus.SC, mps);

                    ModalityProcedureStepType mpsType = CollectionUtils.SelectFirst(rpType.ModalityProcedureStepTypes,
                        delegate(ModalityProcedureStepType mpst) { return mpst.Equals(mps.Type); });

                    Assert.IsNotNull(mpsType, "diagnostic service plan not copied correctly");
                }
            }


            // check that scheduling time was propagated to all requested procedures and procedure steps
            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                Assert.AreEqual(scheduleTime, rp.ScheduledStartTime, "incorrect RP scheduled start time");
                foreach (ProcedureStep ps in rp.ProcedureSteps)
                {
                    Assert.AreEqual(scheduleTime, ps.Scheduling.StartTime, "incorrect PS scheduled start time");
                }
            }
        }

        /// <summary>
        /// Schedule an unscheduled procedure step, verify that order and procedure scheduling
        /// are updated to reflect the scheduled time.
        /// </summary>
        [Test]
        public void ScheduleProcedureStep()
        {
            // create an unscheduled order
            Order order = TestOrderFactory.CreateOrder(1, 1, false);

            DateTime scheduleTime = DateTime.Now;

            // scheduled the check-in step
            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProcedureStep step = CollectionUtils.FirstElement<ProcedureStep>(rp.ProcedureSteps);
            step.Schedule(scheduleTime);

            Assert.AreEqual(scheduleTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time are updated to reflect earliest time
            Assert.AreEqual(scheduleTime, rp.ScheduledStartTime);
            Assert.AreEqual(scheduleTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when all procedure steps are unscheduled, the procedure and order are
        /// also unscheduled.
        /// </summary>
        [Test]
        public void UnscheduleProcedureStep()
        {
            // create a scheduled order
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // unschedule all steps
            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    step.Schedule(null);
                }
                // verify rp start time is null
                Assert.IsNull(rp.ScheduledStartTime);
            }

            // verify order start time is null
            Assert.IsNull(order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when a procedure step is rescheduled, procedure and order scheduling information
        /// is updated to reflect the earlist procedure step.
        /// </summary>
        [Test]
        public void RescheduleEarlier()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            DateTime originalTime = (DateTime)order.ScheduledStartTime;
            DateTime newTime = originalTime - TimeSpan.FromDays(1);

            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProcedureStep step = CollectionUtils.FirstElement<ProcedureStep>(rp.ProcedureSteps);
            step.Schedule(newTime);

            Assert.AreEqual(newTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time are updated to reflect earliest time
            Assert.AreEqual(newTime, rp.ScheduledStartTime);
            Assert.AreEqual(newTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// Verify that when a procedure step is rescheduled later, the order and procedure
        /// scheduling information still reflects the earliest start time.
        /// </summary>
        [Test]
        public void RescheduleLater()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            DateTime originalTime = (DateTime)order.ScheduledStartTime;
            DateTime newTime = originalTime + TimeSpan.FromDays(1);

            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProcedureStep step = CollectionUtils.FirstElement<ProcedureStep>(rp.ProcedureSteps);
            step.Schedule(newTime);

            Assert.AreEqual(newTime, step.Scheduling.StartTime);

            // verify order and rp scheduled time still reflect earliest time
            Assert.AreEqual(originalTime, rp.ScheduledStartTime);
            Assert.AreEqual(originalTime, order.ScheduledStartTime);
        }

        /// <summary>
        /// When an order is cancelled, verify that all procedures are cancelled, and all
        /// procedure steps are discontinued.
        /// </summary>
        [Test]
        public void CancelOrderFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);
            order.Cancel(_defaultCancelReason);

            CheckStatus(OrderStatus.CA, order);
            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                CheckStatus(RequestedProcedureStatus.CA, rp);
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    CheckStatus(ActivityStatus.DC, step);
                }
            }
        }

        /// <summary>
        /// Verify that an order cannot be cancelled after it is already in progress.
        /// </summary>
        [Test]
        public void CancelOrderFromInProgress()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // put the order in progress
            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProcedureStep step = CollectionUtils.FirstElement<ProcedureStep>(rp.ProcedureSteps);
            step.Start(TestStaffFactory.CreateStaff(StaffType.STEC));

            try
            {
                order.Cancel(_defaultCancelReason);

                Assert.Fail("expected exception when trying to cancel non-scheduled order");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(typeof(WorkflowException), e);
            }
        }

        /// <summary>
        /// When an order is discontinued, verify that scheduled procedures are discontinued
        /// but in progress procedures are allowed to complete.
        /// </summary>
        [Test]
        public void DiscontinueOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // copy req procs to a list so we can access them by index
            List<RequestedProcedure> reqProcs = new List<RequestedProcedure>(
                new TypeSafeEnumerableWrapper<RequestedProcedure>(order.RequestedProcedures));
            RequestedProcedure rp1 = reqProcs[0];
            RequestedProcedure rp2 = reqProcs[1];

            // start rp 1
            rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(StaffType.STEC));

            order.Discontinue(_defaultCancelReason);

            // order is discontinued
            CheckStatus(OrderStatus.DC, order);

            // rp 2 is canceled
            CheckStatus(RequestedProcedureStatus.CA, rp2);

            // rp 1 is still in progress
            CheckStatus(RequestedProcedureStatus.IP, rp1);
        }

        /// <summary>
        /// Verify that order is auto-discontinued when all procedures are cancelled.
        /// </summary>
        [Test]
        public void AutoDiscontinueOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // copy req procs to a list so we can access them by index
            List<RequestedProcedure> reqProcs = new List<RequestedProcedure>(
                new TypeSafeEnumerableWrapper<RequestedProcedure>(order.RequestedProcedures));
            RequestedProcedure rp1 = reqProcs[0];
            RequestedProcedure rp2 = reqProcs[1];

            // start and discontinue rp1
            rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(StaffType.STEC));
            rp1.Discontinue();

            // cancel rp2
            rp2.Cancel();

            // order should be discontinued
            CheckStatus(OrderStatus.DC, order);
        }

        /// <summary>
        /// Verify that order is auto-cancelled when all procedures are cancelled.
        /// </summary>
        [Test]
        public void AutoCancelOrder()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                rp.Cancel();
            }

            CheckStatus(OrderStatus.CA, order);
        }

        /// <summary>
        /// Verify that an order and procedure automatically move to the IP status when one of its procedure steps
        /// is started.
        /// </summary>
        [Test]
        public void AutoStartOrderProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            // put the order in progress
            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProcedureStep step = CollectionUtils.FirstElement<ProcedureStep>(rp.ProcedureSteps);
            step.Start(TestStaffFactory.CreateStaff(StaffType.STEC));

            // requested procedure is in progress
            CheckStatus(RequestedProcedureStatus.IP, rp);

            // order is in progress
            CheckStatus(OrderStatus.IP, order);
        }

        /// <summary>
        /// Verify that when a procedure is cancelled, all steps are discontinued.
        /// </summary>
        [Test]
        public void CancelProcedureFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(1, 2, true);

            // copy req procs to a list so we can access them by index
            List<RequestedProcedure> reqProcs = new List<RequestedProcedure>(
                new TypeSafeEnumerableWrapper<RequestedProcedure>(order.RequestedProcedures));
            RequestedProcedure rp1 = reqProcs[0];

            rp1.Cancel();
            CheckStatus(RequestedProcedureStatus.CA, rp1);

            foreach (ProcedureStep step in rp1.ProcedureSteps)
            {
                // all steps were scheduled, so they should all be discontinued
                CheckStatus(ActivityStatus.DC, step);
            }
        }

        /// <summary>
        /// Verify that an in-progress procedure cannot be cancelled.
        /// </summary>
        [Test]
        public void CancelProcedureFromInProgress()
        {
            try
            {
                Order order = TestOrderFactory.CreateOrder(1, 1, true);

                RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
                rp.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(StaffType.STEC));

                CheckStatus(RequestedProcedureStatus.IP, rp);

                rp.Cancel();

                Assert.Fail("expected exception when trying to cancel in progress procedure");

            }
            catch (WorkflowException e)
            {
                Assert.IsInstanceOfType(typeof(WorkflowException), e);
            }
        }

        /// <summary>
        /// Verify that when a procedure is discontinued:
        /// a) SC steps are discontinued
        /// b) IP, CM steps are unchanged.
        /// </summary>
        [Test]
        public void DiscontinueProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(1, 3, true);

            // copy req procs to a list so we can access them by index
            List<RequestedProcedure> reqProcs = new List<RequestedProcedure>(
                new TypeSafeEnumerableWrapper<RequestedProcedure>(order.RequestedProcedures));
            RequestedProcedure rp1 = reqProcs[0];

            // put one mps in progress and the other completed, leaving the third scheduled
            rp1.ModalityProcedureSteps[0].Start(TestStaffFactory.CreateStaff(StaffType.STEC));
            rp1.ModalityProcedureSteps[1].Complete(TestStaffFactory.CreateStaff(StaffType.STEC));

            // discontinue rp1
            rp1.Discontinue();

            CheckStatus(RequestedProcedureStatus.DC, rp1);

            // expect scheduled step was discontinued
            CheckStatus(ActivityStatus.DC, rp1.ModalityProcedureSteps[2]);

            // expect other steps unchanged
            CheckStatus(ActivityStatus.CM, rp1.ModalityProcedureSteps[1]);
            CheckStatus(ActivityStatus.IP, rp1.ModalityProcedureSteps[0]);
        }

        /// <summary>
        /// Verify that a procedure step can be started, and that the order/procedure move to IP status.
        /// </summary>
        [Test]
        public void StartProcedureStep()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);

            RequestedProcedure rp1 = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

            mps1.Start(TestStaffFactory.CreateStaff(StaffType.STEC));

            CheckStatus(ActivityStatus.IP, mps1);
            CheckStatus(RequestedProcedureStatus.IP, rp1);
            CheckStatus(OrderStatus.IP, order);
        }

        /// <summary>
        /// Verify that a procedure step can be completed directly from the SC status,
        /// and that the order/procedure status are unchanged.
        /// </summary>
        [Test]
        public void CompleteProcedureStepFromScheduled()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);

            RequestedProcedure rp1 = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

            mps1.Complete(TestStaffFactory.CreateStaff(StaffType.STEC));

            CheckStatus(ActivityStatus.CM, mps1);
            CheckStatus(RequestedProcedureStatus.IP, rp1);
            CheckStatus(OrderStatus.IP, order);
        }

        /// <summary>
        /// Verify that a procedure step can be completed from the IP status,
        /// and that the order/procedure status are unchanged.
        /// </summary>
        [Test]
        public void CompleteProcedureStepFromInProgress()
        {
            Order order = TestOrderFactory.CreateOrder(1, 1, true);
            RequestedProcedure rp1 = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ModalityProcedureStep mps1 = rp1.ModalityProcedureSteps[0];
            CheckStatus(ActivityStatus.SC, mps1);

            mps1.Start(TestStaffFactory.CreateStaff(StaffType.STEC));

            CheckStatus(ActivityStatus.IP, mps1);

            rp1.ModalityProcedureSteps[0].Complete();

            CheckStatus(ActivityStatus.CM, mps1);
            CheckStatus(RequestedProcedureStatus.IP, rp1);
            CheckStatus(OrderStatus.IP, order);
        }

        /// <summary>
        /// Verify that a procedure step can be discontinued,
        /// and that the parent procedure order status are unchanged
        /// (assuming it is not the only procedure step).
        /// </summary>
        [Test]
        public void DiscontinueProcedureStep()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            CheckStatus(OrderStatus.SC, order);

            RequestedProcedure rp1 = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            CheckStatus(RequestedProcedureStatus.SC, rp1);

            rp1.ModalityProcedureSteps[0].Discontinue();
            CheckStatus(ActivityStatus.DC, rp1.ModalityProcedureSteps[0]);

            // rp and order status unchanged
            CheckStatus(RequestedProcedureStatus.SC, rp1);
            CheckStatus(OrderStatus.SC, order);
            Assert.IsNull(rp1.ProcedureCheckIn.CheckOutTime);
        }

        /// <summary>
        /// Verify that when all procedure steps are discontinued, a procedure is automatically 
        /// discontinued.
        /// </summary>
        [Test]
        public void AutoDiscontinueProcedure()
        {
            Order order = TestOrderFactory.CreateOrder(2, 2, true);

            CheckStatus(OrderStatus.SC, order);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                foreach (ProcedureStep step in rp.ProcedureSteps)
                {
                    step.Discontinue();
                    CheckStatus(ActivityStatus.DC, step);
                }
                CheckStatus(RequestedProcedureStatus.DC, rp);
            }

            CheckStatus(OrderStatus.DC, order);
        }

        #region Helper methods

        private void CheckStatus(OrderStatus status, Order o)
        {
            Assert.AreEqual(status, o.Status, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }
        private void CheckStatus(RequestedProcedureStatus status, RequestedProcedure o)
        {
            Assert.AreEqual(status, o.Status, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }
        private void CheckStatus(ActivityStatus status, ProcedureStep o)
        {
            Assert.AreEqual(status, o.State, string.Format("Exptected {0} status {1}", o.GetClass().Name, status.ToString()));
        }

        #endregion
    }
}

#endif
