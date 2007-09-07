using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class that roughly represents the notion of an IHE/DICOM General Purpose Scheduled Procedure Step (GP-SPS)
    /// </summary>
    public abstract class ProcedureStep : Activity
    {
        private RequestedProcedure _requestedProcedure;

        /// <summary>
        /// No-args constructor required by NHibernate.
        /// </summary>
        public ProcedureStep()
        {
        }

        /// <summary>
        /// Constructor that assigns this step to a parent procedure.
        /// </summary>
        /// <param name="procedure"></param>
        public ProcedureStep(RequestedProcedure procedure)
        {
            _requestedProcedure = procedure;
            procedure.ProcedureSteps.Add(this);
        }

        /// <summary>
        /// Gets a user-friendly descriptive name for this procedure step.
        /// </summary>
        public abstract string Name { get; }


        /// <summary>
        /// Gets the associated requested procedure
        /// </summary>
        public virtual RequestedProcedure RequestedProcedure
        {
            get { return _requestedProcedure; }
            internal set { _requestedProcedure = value; }
        }

        /// <summary>
        /// Assigns the specified staff as the scheduled performer of this step.  Note that this operation is only valid
        /// while the step is in the scheduled state.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Assign(Staff performer)
        {
            Assign(new ProcedureStepPerformer(performer));
        }

        /// <summary>
        /// Gets the staff that this step was assigned to
        /// </summary>
        public virtual Staff AssignedStaff
        {
            get
            {
                return this.Scheduling != null && this.Scheduling.Performer != null ?
                  ((ProcedureStepPerformer)this.Scheduling.Performer).Staff : null;
            }
        }

        /// <summary>
        /// Gets the staff that performed this step, which may not be the same as the <see cref="AssignedStaff"/>
        /// </summary>
        public virtual Staff PerformingStaff
        {
            get
            {
                return this.Performer != null ? ((ProcedureStepPerformer)this.Performer).Staff : null;
            }
        }

        /// <summary>
        /// Starts the step using the specified staff as the performer
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Start(Staff performer)
        {
            Platform.CheckForNullReference(performer, "performer");

            Start(new ProcedureStepPerformer(performer));
        }

        /// <summary>
        /// Completes the step using the specified staff as the performer, assuming a performer has not already been assigned
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Complete(Staff performer)
        {
            Platform.CheckForNullReference(performer, "performer");

            Complete(new ProcedureStepPerformer(performer));
        }

        /// <summary>
        /// Called when the scheduling information for this procedure step has changed.
        /// </summary>
        protected override void OnSchedulingChanged()
        {
            _requestedProcedure.UpdateScheduling();

            base.OnSchedulingChanged();
        }

        /// <summary>
        /// Called after this procedure step undergoes a state transition.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="newState"></param>
        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            _requestedProcedure.UpdateStatus();

            base.OnStateChanged(previousState, newState);
        }
    }
}
