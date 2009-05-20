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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class that roughly represents the notion of an IHE/DICOM General Purpose Scheduled Procedure Step (GP-SPS)
    /// </summary>
    public abstract class ProcedureStep : Activity
    {
		#region Static members

		/// <summary>
		/// Returns all concrete subclasses of this class.
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IList<Type> ListSubClasses(bool includeAbstract, IPersistenceContext context)
		{
			return CollectionUtils.Select(context.GetBroker<IMetadataBroker>().ListEntityClasses(),
				delegate(Type t) { return (includeAbstract || !t.IsAbstract) && t.IsSubclassOf(typeof(ProcedureStep)); });
		}

		/// <summary>
		/// Gets the subclass matching the specified name, which need not be fully qualified.
		/// </summary>
		/// <param name="subclassName"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static Type GetSubClass(string subclassName, IPersistenceContext context)
		{
			return CollectionUtils.SelectFirst(ListSubClasses(true, context),
				delegate(Type t) { return t.FullName.EndsWith(subclassName); });
		}

		#endregion


		private Procedure _procedure;
    	private ProcedureStep _linkStep;

		#region Constructors

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
		public ProcedureStep(Procedure procedure)
		{
			this._procedure = procedure;
			procedure.ProcedureSteps.Add(this);
		}

		#endregion

		#region Public Properties

		/// <summary>
        /// Gets a user-friendly descriptive name for this procedure step.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the associated procedure.
        /// </summary>
        public virtual Procedure Procedure
        {
            get { return _procedure; }
            internal set { _procedure = value; }
        }

		/// <summary>
		/// Gets the entire set of associated procedures, which is the <see cref="Procedure"/> plus any linked procedures
		/// return by <see cref="GetLinkedProcedures"/>.
		/// </summary>
    	public virtual List<Procedure> AllProcedures
    	{
			get { return CollectionUtils.Concat<Procedure>(GetLinkedProcedures(), new Procedure[] { _procedure }); }
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
		/// Indicates if the procedure is a "Pre" procedure step.  If true, the procedure step's 
		/// procedure is not started with the procedure.
		/// </summary>
		public abstract bool IsPreStep { get; }

		/// <summary>
		/// Gets the procedure step to which this step was linked, or null if this step is not linked.
		/// </summary>
    	public virtual ProcedureStep LinkStep
    	{
			get { return _linkStep; }
			private set { _linkStep = value; }
    	}

		/// <summary>
		/// Gets a value indicating whether this step was discontinued and linked to another step.
		/// </summary>
    	public virtual bool IsLinked
    	{
			get { return this.State == ActivityStatus.DC && _linkStep != null; }
    	}

		#endregion

		#region Public methods

        /// <summary>
        /// Assigns the specified staff as the scheduled performer of this step.  Note that this operation is only valid
        /// while the step is in the scheduled state.  The value may be null, in which case the step is unassigned.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Assign(Staff performer)
        {
            Assign(performer == null ? null : new ProcedureStepPerformer(performer));
        }

        /// <summary>
        /// Re-assigns this step to the specified scheduled performer.
        /// </summary>
        /// <remarks>
        /// If this step is currently Scheduled, the scheduled performer is simply changed
        /// to the specified performer.  If this step is in-progress or suspended, then
        /// it is discontinued, and a new step is scheduled and assigned to the specified performer.
        /// </remarks>
        /// <param name="performer"></param>
        /// <returns>A new step with the assigned performer.</returns>
        public virtual ProcedureStep Reassign(Staff performer)
        {
            if(this.IsTerminated)
                throw new WorkflowException("Cannot re-assign a terminated procedure step.");

            if (this.State == ActivityStatus.SC)
            {
                this.Assign(performer);
                return this;
            }
            else
            {
                this.Discontinue();
                ProcedureStep newStep = CreateScheduledCopy();
                newStep.Schedule(this.Scheduling.StartTime, this.Scheduling.EndTime);
                newStep.Assign(performer);
                return newStep;
            }
        }

        /// <summary>
        /// Starts the step using the specified staff as the performer
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Start(Staff performer)
        {
            Start(performer, null);
        }

        /// <summary>
        /// Starts the step using the specified staff as the performer, recording the specified startTime.
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="startTime"></param>
        public virtual void Start(Staff performer, DateTime? startTime)
        {
            Platform.CheckForNullReference(performer, "performer");

            Start(new ProcedureStepPerformer(performer), startTime);
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
		/// Links this step to the specified other step, effectively discontinuing this step.
		/// </summary>
		/// <param name="other"></param>
		public virtual void LinkTo(ProcedureStep other)
		{
			if (this.State != ActivityStatus.SC)
				throw new WorkflowException("Cannot link to another step because this step has already been started.");

			// link the procedure to the specified other step
			other.LinkProcedure(_procedure);

			// record the step that we linked to
			_linkStep = other;

			// discontinue this step so it doesn't show up in any worklists
			this.Discontinue();
		}

		/// <summary>
		/// Gets the set of procedure steps that are related to this procedure step,
		/// in the sense that they relate to the same work artifact (protocol, report, etc).
		/// Note that the result includes this step, because a procedure is by 
		/// definition related to itself.
		/// </summary>
		/// <returns></returns>
		public virtual List<ProcedureStep> GetRelatedProcedureSteps()
		{
			return CollectionUtils.Select(_procedure.ProcedureSteps, IsRelatedStep);
		}


		/// <summary>
		/// Gets any linked procedures that are reachable through this step.
		/// </summary>
		/// <returns></returns>
    	public abstract List<Procedure> GetLinkedProcedures();


		#endregion

		#region Protected API

        /// <summary>
        /// Links the specified procedure to the workflow artifact associated with this step.
        /// </summary>
        /// <param name="procedure"></param>
        protected virtual void LinkProcedure(Procedure procedure)
        {
            throw new WorkflowException(string.Format("Procedure steps of class {0} do not support linking.", this.GetClass().Name));
        }

        /// <summary>
        /// Called when the scheduling information for this procedure step has changed.
        /// </summary>
        protected override void OnSchedulingChanged()
        {
            _procedure.UpdateScheduling();

            base.OnSchedulingChanged();
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
                _procedure.UpdateStatus();
            }

            base.OnStateChanged(previousState, newState);
        }

		/// <summary>
		/// Create a new step of the same type in the Scheduled state.
		/// </summary>
		/// <returns></returns>
		protected abstract ProcedureStep CreateScheduledCopy();

		/// <summary>
		/// Tests whether the specified procedure step is directly related to this step,
		/// in the sense that it targets the same work artifact (eg. protocol, report, etc.)
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		protected abstract bool IsRelatedStep(ProcedureStep step);

		#endregion
	}
}
