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

using System;
using System.Collections.Generic;
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
        #region Static Factory methods

        /// <summary>
        /// Factory method to create a fully initialized new order for the default set of requested procedures
        /// for the specified diagnostic service.
        /// </summary>
        public static Order NewOrder(
            string accessionNumber,
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            string reasonForStudy,
            OrderPriority priority,
            Facility orderingFacility,
            Facility performingFacility,
            DateTime? schedulingRequestTime,
            DateTime? scheduledStartTime,
            ExternalPractitioner orderingPractitioner,
            IList<ExternalPractitioner> copiesToPractitioners,
            IList<OrderAttachment> attachments)
        {
            // create requested procedures according to the diagnostic service breakdown
            IList<RequestedProcedure> procedures = CollectionUtils.Map<RequestedProcedureType, RequestedProcedure>(diagnosticService.RequestedProcedureTypes,
                delegate(RequestedProcedureType type)
                {
                    // each procedure is automatically added to order
                    RequestedProcedure rp = type.CreateProcedure(scheduledStartTime);
                    rp.PerformingFacility = performingFacility;
                    return rp;
                });

            return NewOrder(accessionNumber, patient, visit, diagnosticService, reasonForStudy,
                priority, orderingFacility, schedulingRequestTime, orderingPractitioner, copiesToPractitioners,
                procedures, attachments);
        }


        /// <summary>
        /// Factory method to create a fully initialized new order for the specified requested procedures.
        /// </summary>
        public static Order NewOrder(
            string accessionNumber,
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            string reasonForStudy,
            OrderPriority priority,
            Facility orderingFacility,
            DateTime? schedulingRequestTime,
            ExternalPractitioner orderingPractitioner,
            IList<ExternalPractitioner> copiesToPractitioners,
            IList<RequestedProcedure> procedures,
            IList<OrderAttachment> attachments)
        {
            // create the basic order
            Order order = new Order();

            order.Patient = patient;
            order.Visit = visit;
            order.AccessionNumber = accessionNumber;
            order.DiagnosticService = diagnosticService;
            order.ReasonForStudy = reasonForStudy;
            order.Priority = priority;
            order.OrderingFacility = orderingFacility;
            order.EnteredDateTime = Platform.Time;
            order.SchedulingRequestDateTime = schedulingRequestTime;
            order.OrderingPractitioner = orderingPractitioner;
            order.ResultCopiesToPractitioners.AddAll(copiesToPractitioners);

            // associate all procedures with the order
            foreach (RequestedProcedure rp in procedures)
            {
                order.AddRequestedProcedure(rp);
            }

            foreach (OrderAttachment attachment in attachments)
            {
                order.Attachments.Add(attachment);
            }

            return order;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this order is in a terminal state.
        /// </summary>
        public virtual bool IsTerminated
        {
            get
            {
                return _status == OrderStatus.CM || _status == OrderStatus.CA || _status == OrderStatus.DC;
            }
        }

        public virtual bool CanCompleteDocumentation
        {
            get
            {
                // All mps and mpps must be completed or discontinued
                foreach (RequestedProcedure requestedProcedure in RequestedProcedures)
                {
                    foreach (ModalityProcedureStep modalityProcedureStep in requestedProcedure.ModalityProcedureSteps)
                    {
                        if(modalityProcedureStep.State != ActivityStatus.CM && modalityProcedureStep.State != ActivityStatus.DC)
                        {
                            return false;
                        }

                        foreach (PerformedStep performedStep in modalityProcedureStep.PerformedSteps)
                        {
                            if (performedStep.State == PerformedStepStatus.IP)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        #endregion



        #region Public operations

        /// <summary>
        /// Adds the specified procedure to this order.
        /// </summary>
        /// <param name="rp"></param>
        public virtual void AddRequestedProcedure(RequestedProcedure rp)
        {
            if (rp.Order != null || rp.Status != RequestedProcedureStatus.SC)
                throw new ArgumentException("Only new RequestedProcedure objects may be added to an order.");
            if (this.IsTerminated)
                throw new WorkflowException(string.Format("Cannot add procedure to order with status {0}.", _status));

            rp.Order = this;

            // generate an index for the procedure
            int highestIndex = CollectionUtils.Max<int>(
                CollectionUtils.Map<RequestedProcedure, int>(_requestedProcedures,
                delegate(RequestedProcedure p) { return int.Parse(p.Index); }), 0);
            rp.Index = (highestIndex + 1).ToString();

            // add to collection
            _requestedProcedures.Add(rp);

            // update scheduling information
            UpdateScheduling();
        }

        /// <summary>
        /// Removes the specified procedure from this order.
        /// </summary>
        /// <param name="rp"></param>
        public virtual void RemoveRequestedProcedure(RequestedProcedure rp)
        {
            if(!_requestedProcedures.Contains(rp))
                throw new ArgumentException("Specified requested procedure does not exist for this order.");
            if(rp.Status != RequestedProcedureStatus.SC)
                throw new WorkflowException("Only procedures in the SC status can be removed from an order.");

            _requestedProcedures.Remove(rp);
            rp.Order = null;
        }

        /// <summary>
        /// Cancels the order.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void Cancel(OrderCancelReasonEnum reason)
        {
            if (this.Status != OrderStatus.SC)
                throw new WorkflowException("Only orders in the SC status can be canceled");

            _cancelReason = reason;

            // update the status prior to cancelling the procedures
            // (otherwise cancelling the procedures will cause them to try and update the order status)
            _status = OrderStatus.CA;

            // cancel all procedures
            foreach (RequestedProcedure rp in _requestedProcedures)
            {
                rp.Cancel();
            }
        }

        /// <summary>
        /// Discontinues the order.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void Discontinue(OrderCancelReasonEnum reason)
        {
            if (this.Status != OrderStatus.IP)
                throw new WorkflowException("Only orders in the IP status can be discontinued");

            _cancelReason = reason;

            // update the status prior to cancelling the procedures
            // (otherwise cancelling the procedures will cause them to try and update the order status)
            _status = OrderStatus.DC;
            
            // cancel any scheduled procedures
            foreach (RequestedProcedure rp in _requestedProcedures)
            {
                if (rp.Status == RequestedProcedureStatus.SC)
                    rp.Cancel();
            }
        }

        #endregion

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

        #region Helper methods

        /// <summary>
        /// Called by a child requested procedure to tell the order to update its scheduling information.
        /// </summary>
        protected internal virtual void UpdateScheduling()
        {
            // set the scheduled start time to the earliest non-null scheduled start time of any child procedure
            _scheduledStartTime = CollectionUtils.Min<DateTime?>(
                CollectionUtils.Select<DateTime?>(
                    CollectionUtils.Map<RequestedProcedure, DateTime?>(this.RequestedProcedures,
                        delegate(RequestedProcedure rp) { return rp.ScheduledStartTime; }),
                            delegate(DateTime? startTime) { return startTime != null; }), null);
        }

        /// <summary>
        /// Called by a child requested procedure to tell the order to update its status.  Only
        /// certain status updates can be inferred deterministically from child statuses.  If no
        /// status can be inferred, the status does not change.
        /// </summary>
        protected internal virtual void UpdateStatus()
        {
            // if the order has not yet terminated, it may need to be auto-terminated
            if (!IsTerminated)
            {
                // if all rp are cancelled, the order is cancelled
                if (CollectionUtils.TrueForAll<RequestedProcedure>(_requestedProcedures,
                    delegate(RequestedProcedure rp) { return rp.Status == RequestedProcedureStatus.CA; }))
                {
                    _status = OrderStatus.CA;
                }
                else
                // if all rp are cancelled or discontinued, the order is discontinued
                if (CollectionUtils.TrueForAll<RequestedProcedure>(_requestedProcedures,
                   delegate(RequestedProcedure rp) { return rp.Status == RequestedProcedureStatus.CA || rp.Status == RequestedProcedureStatus.DC; }))
                {
                    _status = OrderStatus.DC;
                }
                else
                // if all rp are cancelled, discontinued or completed, then the order is completed
                if (CollectionUtils.TrueForAll<RequestedProcedure>(_requestedProcedures,
                   delegate(RequestedProcedure rp) { return rp.IsTerminated; }))
                {
                    _status = OrderStatus.CM;
                }
            }

            // if the order is still scheduled, it may need to be auto-started
            if (_status == OrderStatus.SC)
            {
                if (CollectionUtils.Contains<RequestedProcedure>(_requestedProcedures,
                   delegate(RequestedProcedure rp) { return rp.Status == RequestedProcedureStatus.IP || rp.Status == RequestedProcedureStatus.CM; }))
                {
                    _status = OrderStatus.IP;
                }
            }
        }

        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        #endregion
    }
}
