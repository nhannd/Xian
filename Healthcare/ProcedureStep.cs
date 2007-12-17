#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
        /// Indicates if the procedure is a "Pre" procedure step.  If true, the procedure step's requested
        /// procedure is not started with the procedure.
        /// </summary>
        protected virtual bool IsPreStep
        {
            get { return false; }
        }

        /// <summary>
        /// Called after this procedure step undergoes a state transition.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="newState"></param>
        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if (this.IsPreStep == false)
            {
                _requestedProcedure.UpdateStatus();
            }

            base.OnStateChanged(previousState, newState);
        }
    }
}
