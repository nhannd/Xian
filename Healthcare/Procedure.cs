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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{


    /// <summary>
    /// Procedure entity
    /// </summary>
    public partial class Procedure : Entity
    {
        public Procedure(ProcedureType type)
        {
            _type = type;
            _procedureSteps = new HashedSet<ProcedureStep>();
            _procedureCheckIn = new ProcedureCheckIn();
            _reports = new HashedSet<Report>();
            _protocols = new HashedSet<Protocol>();
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
                    CollectionUtils.Select(this.ProcedureSteps,
                        delegate(ProcedureStep ps)
                        {
                            return ps.Is<ModalityProcedureStep>();
                        }), delegate(ProcedureStep ps) { return ps.As<ModalityProcedureStep>(); });
            }
        }

        /// <summary>
        /// Gets the reporting procedure steps.
        /// </summary>
        public virtual List<ReportingProcedureStep> ReportingProcedureSteps
        {
            get
            {
                return CollectionUtils.Map<ProcedureStep, ReportingProcedureStep>(
                    CollectionUtils.Select(this.ProcedureSteps,
                        delegate(ProcedureStep ps)
                        {
                            return ps.Is<ReportingProcedureStep>();
                        }), delegate(ProcedureStep ps) { return ps.As<ReportingProcedureStep>(); });
            }
        }

        /// <summary>
        /// Gets a value indicating whether this procedure is in a terminal state.
        /// </summary>
        public virtual bool IsTerminated
        {
            get
            {
                return _status == ProcedureStatus.CM || _status == ProcedureStatus.CA || _status == ProcedureStatus.DC;
            }
        }

        /// <summary>
        /// Gets the documentation procedure step, or null if it does not exist.
        /// </summary>
        public virtual DocumentationProcedureStep DocumentationProcedureStep
        {
            get
            {
                ProcedureStep step = CollectionUtils.SelectFirst(this.ProcedureSteps,
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
        /// Gets the active <see cref="Protocol"/> for this procedure, or returns null if there is no active protocol.
        /// </summary>
        public virtual Protocol ActiveProtocol
        {
            get
            {
                return CollectionUtils.SelectFirst(_protocols,
                    delegate(Protocol p) { return p.Status != ProtocolStatus.X; });
            }
        }

        /// <summary>
        /// Gets the time at which the procedure can be considered "performed", which corresponds to the maximum
        /// completed modality procedure step end-time.
        /// </summary>
        /// <remarks>
        /// This is a computed property and hence should not be used in summaries.
        /// </remarks>
        public virtual DateTime? PerformedTime
        {
            get
            {
                List<ModalityProcedureStep> completedSteps = CollectionUtils.Select(this.ModalityProcedureSteps,
                    delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.CM; });

                // return the max end-time over all completed MPS
                ModalityProcedureStep maxMps = CollectionUtils.Max(completedSteps, null,
                    delegate(ModalityProcedureStep mps1, ModalityProcedureStep mps2)
                    {
                        return Nullable.Compare(mps1.EndTime, mps2.EndTime);
                    });

                return maxMps != null ? maxMps.EndTime : null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this procedure can be considered "performed", meaning all modality procedure
        /// steps have been terminated, and at least one has been completed.
        /// </summary>
        /// <remarks>
        /// This is a computed property and hence should not be used in summaries.
        /// </remarks>
        public virtual bool IsPerformed
        {
            get
            {
                // return true if all MPS are terminated and at least one is completed
                return this.ModalityProcedureSteps.TrueForAll(
                        delegate(ModalityProcedureStep mps) { return mps.IsTerminated; })
                    && this.ModalityProcedureSteps.Exists(
                        delegate(ModalityProcedureStep ps) { return ps.State == ActivityStatus.CM; });
            }
        }

        /// <summary>
        /// Gets a value indicating if the order associated to this procedure is ready for reporting.
        /// </summary>
        /// <remarks>
        /// The order would be considered not ready if not all of the procedures are complete, or if the documentatin step is 
        /// incomplete.
        /// </remarks>
        public virtual bool IsNotReadyForReporting
        {
            get { return this.NotReadyForReportingReason != string.Empty; }
        }

        /// <summary>
        /// Gets a value indicating why the order associated to this procedure is not ready for reporting.
        /// </summary>
        /// <remarks>
        /// The order would be considered not ready if not all of the procedures are complete, or if the documentatin step is 
        /// incomplete.
        /// </remarks>
        public virtual string NotReadyForReportingReason
        {
            get
            {
            	string message = string.Empty;
                if (!CollectionUtils.TrueForAll(this.Order.Procedures, delegate(Procedure p) { return p.IsPerformed; }))
                {
                    message = SR.MessageNotAllProceduresComplete;
                }
                else if (this.DocumentationProcedureStep.State != ActivityStatus.CM)
                {
                    message = SR.MessageDocumentationIncomplete;
                }
                return message;
            }
        }

        #endregion

        #region Public Operations

        /// <summary>
        /// Gets all procedure steps matching the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<ProcedureStep> GetProcedureSteps(Predicate<ProcedureStep> predicate)
        {
            return CollectionUtils.Select(_procedureSteps, predicate);
        }

        /// <summary>
        /// Gets the first procedure step matching the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual ProcedureStep GetProcedureStep(Predicate<ProcedureStep> predicate)
        {
            return CollectionUtils.SelectFirst(_procedureSteps, predicate);
        }

        /// <summary>
        /// Creates the procedure steps specified in the procedure plan of the associated
        /// <see cref="ProcedureType"/>.
        /// </summary>
        public virtual void CreateProcedureSteps()
        {
            // TODO: is this the right way to check this condition?  do we need a dedicated flag?
            if (_procedureSteps.Count > 0)
                throw new WorkflowException("Procedure steps have already been created for this Procedure.");

            ProcedureBuilder builder = new ProcedureBuilder();
            builder.BuildProcedureFromPlan(this);
        }

        /// <summary>
        /// Adds a procedure step.  Use this method rather than adding directly to the <see cref="ProcedureSteps"/>
        /// collection.
        /// </summary>
        /// <param name="step"></param>
        public virtual void AddProcedureStep(ProcedureStep step)
        {
            if ((step.Procedure != null && step.Procedure != this) || step.State != ActivityStatus.SC)
                throw new ArgumentException("Only new ProcedureStep objects may be added to an order.");

            step.Procedure = this;
            this.ProcedureSteps.Add(step);
        }

        /// <summary>
        /// Schedules or re-schedules all procedure steps to start at the specified time.
        /// Applicable only if this object is in the SC status.
        /// </summary>
        /// <param name="startTime"></param>
        public virtual void Schedule(DateTime? startTime)
        {
            if (_status != ProcedureStatus.SC)
                throw new WorkflowException("Only procedures in the SC status may be scheduled or re-scheduled.");

            // if the procedure steps have not been created, create them now
            if (_procedureSteps.Count == 0)
            {
                this.CreateProcedureSteps();
            }

            // if we had more detailed scheduling information available in the procedure plan,
            // then we could schedule each step in a more fine-grained manner
            // but since we don't have this information, just schedule each procedure step for the same time
            foreach (ProcedureStep ps in _procedureSteps)
            {
                ps.Schedule(startTime);
            }
        }

        /// <summary>
        /// Discontinue this procedure and any non-terminated procedure steps.
        /// </summary>
        public virtual void Discontinue()
        {
            if (_status != ProcedureStatus.IP)
                throw new WorkflowException("Only procedures in the IP status can be discontinued");

            // update the status prior to cancelling the procedure steps
            // (otherwise cancelling the steps will cause them to try and update the procedure status)
            SetStatus(ProcedureStatus.DC);

            // discontinue any non-terminated procedure steps
            foreach (ProcedureStep ps in _procedureSteps)
            {
                if (!ps.IsTerminated)
                    ps.Discontinue();
            }

            // need to update the end-time again, after discontinuing procedure steps
            UpdateEndTime();
        }

        /// <summary>
        /// Cancel this procedure and all procedure steps.
        /// </summary>
        public virtual void Cancel()
        {
            if (_status != ProcedureStatus.SC)
                throw new WorkflowException("Only procedures in the SC status can be cancelled");

            // update the status prior to cancelling the procedure steps
            // (otherwise cancelling the steps will cause them to try and update the procedure status)
            SetStatus(ProcedureStatus.CA);

            // discontinue all procedure steps (they should all be in the SC status)
            foreach (ProcedureStep ps in _procedureSteps)
            {
                // except PreSteps, which may already be in a terminal state, so ignore them if that's the case.
                // Bug: #1525
                if (ps.IsPreStep && ps.IsTerminated)
                    continue;

                ps.Discontinue();
            }

            // need to update the end-time again, after discontinuing procedure steps
            UpdateEndTime();
        }

        /// <summary>
        /// Gets the full history of this procedure, including procedure steps that 
        /// are associated indirectly via linked workflows.
        /// </summary>
        /// <returns></returns>
        public virtual List<ProcedureStep> GetWorkflowHistory()
        {
            List<ProcedureStep> x = new List<ProcedureStep>(_procedureSteps);
            List<ProcedureStep> history = new List<ProcedureStep>(x);
            while(x.Count > 0)
            {
                // obtain all procedure steps that are linked via steps in x
                List<ProcedureStep> y = 
                    CollectionUtils.Concat<ProcedureStep>(
                        CollectionUtils.Map<ProcedureStep, List<ProcedureStep>>(
                            x,
                            delegate(ProcedureStep step)
                            {
                                return step.IsLinked ? step.LinkStep.GetRelatedProcedureSteps() : new List<ProcedureStep>();
                            }).ToArray());

                history.AddRange(y);

                // set x = y so that the next time through the loop,
                // we follow the next level of linking
                x = y;
            }
            return history;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Called by a child procedure step to complete this procedure.
        /// </summary>
        protected internal virtual void Complete(DateTime completeTime)
        {
            if (_status != ProcedureStatus.IP)
                throw new WorkflowException("Only procedures in the IP status can be completed");

            SetStatus(ProcedureStatus.CM);

            // over-write the end-time with actual completed time
            // TODO: this is a bit of a hack to deal with linked procedures, and the fact that
            // the final ProcedureStep may not exist in our ProcedureSteps collection, 
            // if this procedure was linked to another for reporting.
            // Ideally we should get rid of this at some point, when we build in better tracking
            // of procedure linkages
            _endTime = completeTime;
        }

        /// <summary>
        /// Called by child procedure steps to tell this procedure to update its scheduling information.
        /// </summary>
        protected internal virtual void UpdateScheduling()
        {
            // compute the earliest procedure step scheduled start time (exclude pre-steps)
            _scheduledStartTime = MinMaxHelper.MinValue<ProcedureStep, DateTime?>(_procedureSteps,
                delegate(ProcedureStep ps) { return !ps.IsPreStep; },
                delegate(ProcedureStep ps) { return ps.Scheduling == null ? null : ps.Scheduling.StartTime; }, null);

            // the order should never be null, unless this is a brand new instance that has not yet been assigned an order
            if (_order != null)
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
            if (_status == ProcedureStatus.SC || _status == ProcedureStatus.IP)
            {
                // if all steps are discontinued, this procedure is automatically discontinued
                // Bug: #2471 only consider Modality Procedure Steps for now, although in the long run this is not a good solution
                if (CollectionUtils.TrueForAll(this.ModalityProcedureSteps,
                    delegate(ModalityProcedureStep step) { return step.State == ActivityStatus.DC; }))
                {
                    SetStatus(ProcedureStatus.DC);
                }
            }

            // check if the procedure should be auto-started
            if (_status == ProcedureStatus.SC)
            {
                // the condition for auto-starting the procedure is that it has a (non-pre) procedure step that has
                // moved out of the scheduled status but not into the discontinued status
                bool anyStepStartedNotDiscontinued = CollectionUtils.Contains(_procedureSteps,
                    delegate(ProcedureStep step)
                    {
                        return !step.IsPreStep && !step.IsInitial && step.State != ActivityStatus.DC;
                    });

                if (anyStepStartedNotDiscontinued)
                {
                    SetStatus(ProcedureStatus.IP);
                }
            }
        }

        /// <summary>
        /// Shifts the object in time by the specified number of minutes, which may be negative or positive.
        /// </summary>
        /// <remarks>
        /// The method is not intended for production use, but is provided for the purpose
        /// of generating back-dated data for demos and load-testing.
        /// </remarks>
        /// <param name="minutes"></param>
        protected internal virtual void TimeShift(int minutes)
        {
            _scheduledStartTime = _scheduledStartTime.HasValue ? _scheduledStartTime.Value.AddMinutes(minutes) : _scheduledStartTime;
            _startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
            _endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;

            if (_procedureCheckIn != null)
            {
                _procedureCheckIn.TimeShift(minutes);
            }

            foreach (Protocol protocol in _protocols)
            {
                protocol.TimeShift(minutes);
            }

            foreach (ProcedureStep step in _procedureSteps)
            {
                step.TimeShift(minutes);
            }

            foreach (Report report in _reports)
            {
                report.TimeShift(minutes);
            }
        }

        /// <summary>
        /// Helper method to change the status and also notify the parent order to change its status
        /// if necessary.
        /// </summary>
        /// <param name="status"></param>
        private void SetStatus(ProcedureStatus status)
        {
            if (status != _status)
            {
                _status = status;

                if (_status == ProcedureStatus.IP)
                    UpdateStartTime();

                if (this.IsTerminated)
                    UpdateEndTime();

                // Cancelled/discontinued procedures should not be left in downtime recovery mode.
                if (_status == ProcedureStatus.CA || _status == ProcedureStatus.DC)
                    this.DowntimeRecoveryMode = false;

                // the order should never be null, unless this is a brand new instance that has not yet been assigned an order
                if (_order != null)
                    _order.UpdateStatus();
            }
        }

        private void UpdateStartTime()
        {
            // compute the earliest procedure step start time
            _startTime = MinMaxHelper.MinValue<ProcedureStep, DateTime?>(_procedureSteps,
                delegate(ProcedureStep ps) { return !ps.IsPreStep; },
                delegate(ProcedureStep ps) { return ps.StartTime; }, null);
        }

        private void UpdateEndTime()
        {
            // compute the latest procedure step end time
            _endTime = MinMaxHelper.MaxValue<ProcedureStep, DateTime?>(_procedureSteps,
                delegate(ProcedureStep ps) { return true; },
                delegate(ProcedureStep ps) { return ps.EndTime; }, null);
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
