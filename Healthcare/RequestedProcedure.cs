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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RequestedProcedure entity
    /// </summary>
	public partial class RequestedProcedure : Entity
	{
        public RequestedProcedure(RequestedProcedureType type)
        {
            _type = type;
            _procedureSteps = new HashedSet<ProcedureStep>();
            _procedureCheckIn = new ProcedureCheckIn();
        }
	
        #region Public Properties

        /// <summary>
        /// Gets the modality procedure steps.
        /// </summary>
        public virtual List<ModalityProcedureStep> ModalityProcedureSteps
        {
            get
            {
                return CollectionUtils.Map<ProcedureStep, ModalityProcedureStep>(
                    CollectionUtils.Select<ProcedureStep>(this.ProcedureSteps,
                        delegate(ProcedureStep ps)
                        {
                            return ps.Is<ModalityProcedureStep>();
                        }), delegate(ProcedureStep ps) { return ps.As<ModalityProcedureStep>(); });
            }
        }

        /// <summary>
        /// Gets a value indicating whether this procedure is in a terminal state.
        /// </summary>
        public virtual bool IsTerminated
        {
            get
            {
                return _status == RequestedProcedureStatus.CM || _status == RequestedProcedureStatus.CA || _status == RequestedProcedureStatus.DC;
            }
        }

        public virtual DocumentationProcedureStep DocumentationProcedureStep
        {
            get
            {
                ProcedureStep step = CollectionUtils.SelectFirst<ProcedureStep>(this.ProcedureSteps,
                    delegate(ProcedureStep ps)
                    {
                        return ps.Is<DocumentationProcedureStep>();
                    });

                return step == null ? null : step.Downcast<DocumentationProcedureStep>();
            }
        }

        /// <summary>
        /// Gets the active <see cref="Report"/> for this procedure, or returns null if there is no active report.
        /// </summary>
        public virtual Report ActiveReport
        {
            get
            {
                return CollectionUtils.SelectFirst(_reports,
                    delegate(Report r) { return r.Status != ReportStatus.X; });
            }
        }

        /// <summary>
        /// Gets the active <see cref="ReportingProcedureStep"/> for this procedure, or returns null if
        /// there is no active reporting step.
        /// </summary>
        /// <remarks>
        /// Unlike modality procedure steps, at most one reporting procedure step should ever be in an active status
        /// at any given time.
        /// </remarks>
        public virtual ReportingProcedureStep ActiveReportingStep
        {
            get
            {
                ProcedureStep step = CollectionUtils.SelectFirst(_procedureSteps,
                    delegate(ProcedureStep ps) { return ps.Is<ReportingProcedureStep>() && !ps.IsTerminated; });
                return step != null ? step.As<ReportingProcedureStep>() : null;
            }
        }

        #endregion

        #region Public Operations

        /// <summary>
        /// Adds a procedure step.  Use this method rather than adding directly to the <see cref="ProcedureSteps"/>
        /// collection.
        /// </summary>
        /// <param name="step"></param>
        public virtual void AddProcedureStep(ProcedureStep step)
        {
            if ((step.RequestedProcedure != null && step.RequestedProcedure != this) || step.State != ActivityStatus.SC)
                throw new ArgumentException("Only new ProcedureStep objects may be added to an order.");

            step.RequestedProcedure = this;
            this.ProcedureSteps.Add(step);
        }

        /// <summary>
        /// Schedules or re-schedules all procedure steps to start at the specified time.
        /// Applicable only if this object is in the SC status.
        /// </summary>
        /// <param name="startTime"></param>
        public virtual void Schedule(DateTime? startTime)
        {
            if(_status != RequestedProcedureStatus.SC)
                throw new WorkflowException("Only procedures in the SC status may be scheduled or re-scheduled.");

            // if we had more detailed scheduling information available in the procedure plan,
            // then we could schedule each step in a more fine-grained manner
            // but since we don't have this information, just schedule each procedure step for the same time
            foreach (ProcedureStep ps in _procedureSteps)
            {
                ps.Schedule(startTime);
            }
        }

        /// <summary>
        /// Discontinue this procedure and any procedure steps in the scheduled state.
        /// </summary>
        public virtual void Discontinue()
        {
            if (_status != RequestedProcedureStatus.IP)
                throw new WorkflowException("Only procedures in the IP status can be discontinued");

            // update the status prior to cancelling the procedure steps
            // (otherwise cancelling the steps will cause them to try and update the procedure status)
            SetStatus(RequestedProcedureStatus.DC);
            
            // discontinue any procedure steps in the scheduled status
            foreach (ProcedureStep ps in _procedureSteps)
            {
                if (ps.State == ActivityStatus.SC)
                    ps.Discontinue();
            }
        }

        /// <summary>
        /// Cancel this procedure and all procedure steps.
        /// </summary>
        public virtual void Cancel()
        {
            if (_status != RequestedProcedureStatus.SC)
                throw new WorkflowException("Only procedures in the SC status can be cancelled");

            // update the status prior to cancelling the procedure steps
            // (otherwise cancelling the steps will cause them to try and update the procedure status)
            SetStatus(RequestedProcedureStatus.CA);

            // discontinue all procedure steps (they should all be in the SC status)
            foreach (ProcedureStep ps in _procedureSteps)
            {
                ps.Discontinue();
            }
        }

        #endregion

        #region Object overrides

        public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

        #region Helper methods

        /// <summary>
        /// Called by a child procedure step to complete this procedure.
        /// </summary>
        protected internal virtual void Complete()
        {
            if (_status != RequestedProcedureStatus.IP)
                throw new WorkflowException("Only procedures in the IP status can be completed");

            SetStatus(RequestedProcedureStatus.CM);
        }

        /// <summary>
        /// Called by child procedure steps to tell this procedure to update its scheduling information.
        /// </summary>
        protected internal virtual void UpdateScheduling()
        {
            // compute the earliest procedure step scheduled start time
            _scheduledStartTime = CollectionUtils.Min<DateTime?>(
                CollectionUtils.Select<DateTime?>(
                    CollectionUtils.Map<ProcedureStep, DateTime?>(this.ProcedureSteps,
                    delegate(ProcedureStep step) { return step.Scheduling == null ? null : step.Scheduling.StartTime; }),
                            delegate(DateTime? startTime) { return startTime != null; }), null);

            // the order should never be null, unless this is a brand new instance that has not yet been assigned an order
            if(_order != null)
                _order.UpdateScheduling();
        }

        /// <summary>
        /// Called by a child procedure step to tell the procedure to update its status.  Only
        /// certain status updates can be inferred deterministically from child statuses.  If no
        /// status can be inferred, the status does not change.
        /// </summary>
        protected internal virtual void UpdateStatus()
        {
            // check if the procedure should be auto-discontinued
            if (_status == RequestedProcedureStatus.SC || _status == RequestedProcedureStatus.IP)
            {
                // if all steps are discontinued, this procedure is automatically discontinued
                if (CollectionUtils.TrueForAll<ProcedureStep>(_procedureSteps,
                    delegate(ProcedureStep step) { return step.State == ActivityStatus.DC; }))
                {
                    SetStatus(RequestedProcedureStatus.DC);
                }
            }

            // check if the procedure should be auto-started
            if (_status == RequestedProcedureStatus.SC)
            {
                // the condition for auto-starting the procedure is that it has a procedure step that has
                // moved out of the scheduled status but not into the discontinued status
                bool anyStepStartedNotDiscontinued = CollectionUtils.Contains<ProcedureStep>(_procedureSteps,
                    delegate(ProcedureStep step)
                    {
                        return !step.IsInitial && step.State != ActivityStatus.DC;
                    });

                if (anyStepStartedNotDiscontinued)
                {
                    SetStatus(RequestedProcedureStatus.IP);
                }
            }

            // Update ProcedureCheckIn
            bool allMpsScheduled = this.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.SC; });

            bool allMpsComplete = this.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.CM; });

            bool allMpsTerminated = this.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });

            if (allMpsComplete || allMpsTerminated)
            {
                this.ProcedureCheckIn.CheckOut();
            }
            else if (!allMpsScheduled)
            {
                // check-in this procedure, since some mps has started
                this.ProcedureCheckIn.CheckIn();
            }
        
        }

        /// <summary>
        /// Helper method to change the status and also notify the parent order to change its status
        /// if necessary.
        /// </summary>
        /// <param name="status"></param>
        private void SetStatus(RequestedProcedureStatus status)
        {
            _status = status;

            // the order should never be null, unless this is a brand new instance that has not yet been assigned an order
            if (_order != null)
                _order.UpdateStatus();
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
