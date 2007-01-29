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

        public ProcedureStep()
        {
        }

        public ProcedureStep(RequestedProcedure procedure)
        {
            _requestedProcedure = procedure;
        }

        public abstract string Name { get; }


        /// <summary>
        /// Gets or sets the associated requested procedure
        /// </summary>
        public virtual RequestedProcedure RequestedProcedure
        {
            get { return _requestedProcedure; }
            set { _requestedProcedure = value; }
        }

        /// <summary>
        /// Assigns the specified staff as the scheduled performer of this step.  Note that this operation is only valid
        /// while the step is in the scheduled state.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Assign(Staff performer)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Only procedure steps in the scheduled status can be assigned");

            this.Scheduling.Performer = new ProcedureStepPerformer(performer);
        }

        /// <summary>
        /// Gets the staff that this step was assigned to
        /// </summary>
        public virtual Staff AssignedStaff
        {
            get
            {
                return this.Scheduling.Performer != null ?
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
    }
}
