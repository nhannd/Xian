using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Order entity
    /// </summary>
	public partial class Order : Entity
	{
        public static Order NewOrder(
            string accessionNumber,
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            DateTime schedulingRequestDateTime,
            ExternalPractitioner orderingPhysician,
            Facility orderingFacility,
            OrderPriority priority)
        {
            return Order.NewOrder(accessionNumber, 
                patient, 
                visit, 
                diagnosticService, 
                schedulingRequestDateTime, 
                orderingPhysician, 
                orderingFacility, 
                priority, 
                false);
        }

        public static Order NewOrder(
            string accessionNumber,
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            DateTime schedulingRequestDateTime,
            ExternalPractitioner orderingPhysician,
            Facility orderingFacility,
            OrderPriority priority,
            bool scheduleOrder)
        {
            // create the basic order
            Order order = new Order();

            order.Patient = patient;
            order.Visit = visit;
            order.AccessionNumber = accessionNumber;
            order.DiagnosticService = diagnosticService;
            order.EnteredDateTime = Platform.Time;
            order.SchedulingRequestDateTime = schedulingRequestDateTime;
            order.OrderingPractitioner = orderingPhysician;
            order.OrderingFacility = orderingFacility;
            order.Priority = priority;

            // add requested procedures according to the diagnostic service breakdown
            int rpIndex = 0;
            foreach (RequestedProcedureType rpt in diagnosticService.RequestedProcedureTypes)
            {
                RequestedProcedure rp = new RequestedProcedure(order, rpt, string.Format("{0}", ++rpIndex));

                // Created and scheduled a checkin step for each RP, each RP should only ever has one CPS
                CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                cps.Scheduling.StartTime = schedulingRequestDateTime;
                rp.ProcedureSteps.Add(cps);

                order.RequestedProcedures.Add(rp);

                // add scheduled procedure steps according to the diagnostic service breakdown
                foreach (ModalityProcedureStepType spt in rpt.ModalityProcedureStepTypes)
                {
                    ModalityProcedureStep sps = new ModalityProcedureStep(rp, spt, spt.DefaultModality);

                    if (scheduleOrder)
                    {
                        sps.Scheduling.StartTime = schedulingRequestDateTime;
                        if (order.EarliestScheduledDateTime == null || order.EarliestScheduledDateTime.Value.CompareTo(sps.Scheduling.StartTime) > 0)
                            order.EarliestScheduledDateTime = sps.Scheduling.StartTime;
                    }

                    rp.ProcedureSteps.Add(sps);
                }
            }

            return order;
        }


	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Cancels the order.
        /// </summary>
        /// <param name="reason"></param>
        public void Cancel(OrderCancelReasonEnum reason)
        {
            if (this.Status == OrderStatus.SC)
            {

                this.Status = OrderStatus.CA;
                this.CancelReason = reason;

                DiscontinuePendingProcedureSteps();
            }
            else
            {
                throw new HealthcareWorkflowException("Only orders in the SC status can be canceled");
            }
        }

        /// <summary>
        /// Discontinues the order.
        /// </summary>
        /// <param name="reason"></param>
        public void Discontinue(OrderCancelReasonEnum reason)
        {
            if (this.Status == OrderStatus.IP)
            {
                this.Status = OrderStatus.DC;
                this.CancelReason = reason;

                DiscontinuePendingProcedureSteps();
            }
            else
            {
                throw new HealthcareWorkflowException("Only orders in the IP status can be discontinued");
            }
        }

        private void DiscontinuePendingProcedureSteps()
        {
            foreach (RequestedProcedure rp in this.RequestedProcedures)
            {
                foreach (ProcedureStep ps in rp.ProcedureSteps)
                {
                    if (!ps.IsTerminated)
                    {
                        ps.Discontinue();
                    }
                }
            }
        }

        public bool IsMPSStarted
        {
            get
            {
                return CollectionUtils.Contains<RequestedProcedure>(this.RequestedProcedures,
                    delegate(RequestedProcedure rp)
                    {
                        return rp.IsMpsStarted == true;
                    });
            }
        }

        public bool IsAllRequestedProcedureScheduled
        {
            get
            {
                return CollectionUtils.TrueForAll<RequestedProcedure>(this.RequestedProcedures,
                    delegate(RequestedProcedure rp)
                    {
                        return rp.Status == ActivityStatus.SC;
                    });
            }
        }

        public bool IsAllRequestedProcedureDiscontinued
        {
            get
            {
                return CollectionUtils.TrueForAll<RequestedProcedure>(this.RequestedProcedures,
                    delegate(RequestedProcedure rp)
                    {
                        return rp.Status == ActivityStatus.DC;
                    });
            }
        }


        public bool IsAllRequestedProcedureCompletedOrDiscontinued
        {
            get
            {
                return CollectionUtils.TrueForAll<RequestedProcedure>(this.RequestedProcedures,
                    delegate(RequestedProcedure rp)
                    {
                        return rp.Status == ActivityStatus.DC || rp.Status == ActivityStatus.CM;
                    });
            }
        }

        #region Object overrides
		
		public override bool Equals(object that)
		{
            Order other = that as Order;
			return other != null && other.AccessionNumber == this.AccessionNumber;
		}
		
		public override int GetHashCode()
		{
            return this.AccessionNumber.GetHashCode();
		}
		
		#endregion

	}
}