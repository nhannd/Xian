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
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using Iesi.Collections.Generic;

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Base class for a workflow activity.  An activity is the basic unit of workflow.
    /// Note: this class has been coded for compatability with NHibernate mapping.
    /// </summary>
    public abstract class Activity : PersistentFsm<ActivityStatus>
    {
        private ISet<PerformedStep> _performedSteps;
        private ActivityScheduling _scheduling;
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public Activity()
            :this(new ActivityStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Protected constructor that allows for customized FSM logic.  Typically there should be no need to customize the FSM logic.
        /// </summary>
        /// <param name="transitionLogic"></param>
        protected Activity(IFsmTransitionLogic<ActivityStatus> transitionLogic)
            : base(ActivityStatus.SC, transitionLogic)
        {
            _performedSteps = new HashedSet<PerformedStep>();
            _scheduling = new ActivityScheduling();
        }

        /// <summary>
        /// Gets the set of associated performed steps.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="AddPerformedStep"/> and <see cref="RemovePerformedStep"/> methods.
        /// </summary>
        public virtual ISet<PerformedStep> PerformedSteps
        {
            get { return _performedSteps; }
        }

        /// <summary>
        /// Gets the scheduling for this activity.  May return null, in which case the activity has
        /// no scheduling information.
        /// </summary>
        public virtual ActivityScheduling Scheduling
        {
            get { return _scheduling; }
            private set { _scheduling = value; }
        }


        /// <summary>
        /// Gets the performer of this step.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual ActivityPerformer Performer
        {
            get { return _performer; }
            private set { _performer = value; }
        }

        /// <summary>
        /// Gets the time that this activity was started, which corresponds to the first transition
        /// to the <see cref="ActivityStatus.IP"/> state.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime? StartTime
        {
            get { return _startTime; }
            private set { _startTime = value; }
        }

        /// <summary>
        /// Gets the time at which this activity was terminated, which corresponds to the first transition
        /// to either the <see cref="ActivityStatus.CM"/> or <see cref="ActivityStatus.DC"/> states.
        /// </summary>
        public virtual DateTime? EndTime
        {
            get { return _endTime; }
            private set { _endTime = value; }
        }

        /// <summary>
        /// Schedules or re-schedules the activity at the specified start time. The end time will be
        /// set to null.
        /// </summary>
        /// <param name="startTime"></param>
        public virtual void Schedule(DateTime? startTime)
        {
            Schedule(startTime, null);
        }

        /// <summary>
        /// Schedules or re-schedules the activity at the specified start time. The end time is optional.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public virtual void Schedule(DateTime? startTime, DateTime? endTime)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Schedule is only allowed from Scheduled state");

            if (_scheduling == null)
                _scheduling = new ActivityScheduling();

            this.Scheduling.StartTime = startTime;
            this.Scheduling.EndTime = endTime;

            OnSchedulingChanged();
        }

        public virtual void Assign(ActivityPerformer performer)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Assignment of scheduled performer only allowed from scheduled state");

            if (_scheduling == null)
                _scheduling = new ActivityScheduling();

            this.Scheduling.Performer = performer;

            OnSchedulingChanged();
        }

        /// <summary>
        /// Starts the activity, setting the state to <see cref="ActivityStatus.IP"/> and recording the specified performer.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Start(ActivityPerformer performer)
        {
            Platform.CheckForNullReference(performer, "performer");

            _performer = performer;
            _startTime = Platform.Time;
            ChangeState(ActivityStatus.IP);
        }

        /// <summary>
        /// Suspends the activity, setting the state to <see cref="ActivityStatus.SU"/>
        /// </summary>
        public virtual void Suspend()
        {
            ChangeState(ActivityStatus.SU);
        }

        /// <summary>
        /// Suspends the activity, setting the state to <see cref="ActivityStatus.IP"/>.
        /// Note that the activity must be in the <see cref="ActivityStatus.SU"/> state or an exception will be thrown.
        /// </summary>
        public virtual void Resume()
        {
            if (this.State != ActivityStatus.SU)
                throw new WorkflowException("Resume is only allowed from Suspended state");

            ChangeState(ActivityStatus.IP);
        }

        /// <summary>
        /// Discontinues the activity, setting the state to <see cref="ActivityStatus.DC"/>.
        /// </summary>
        public virtual void Discontinue()
        {
            _endTime = Platform.Time;
            ChangeState(ActivityStatus.DC);
        }

        /// <summary>
        /// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>.  An exception will be thrown
        /// if the performer was not previously specified in the <see cref="Start"/> method.
        /// </summary>
        public virtual void Complete()
        {
            if (_performer == null)
                throw new WorkflowException("Performer must be assigned");

            _endTime = Platform.Time;
            ChangeState(ActivityStatus.CM);
        }

        /// <summary>
        /// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>.  This overload
        /// allows the performer to be specified, which is necessary if the activity is being completed directly
        /// from the scheduled state, and hence the performer was not previously established.  Note that if a performer
        /// has been previously established, and the specified performer is different, an exception will be thrown.
        /// </summary>
        public virtual void Complete(ActivityPerformer performer)
        {
            Platform.CheckForNullReference(performer, "performer");
            if (_performer == null)
            {
                _performer = performer;
            }
            else if (!_performer.Equals(performer))
            {
                throw new WorkflowException("Peformer already assigned");
            }

            this.Complete();
        }

        /// <summary>
        /// Adds the specified performed step to the set of <see cref="PerformedStep"/> objects associated with this activity,
        /// and adds this activity to the set of activities associated with the specified performed step.
        /// </summary>
        /// <param name="step"></param>
        public virtual void AddPerformedStep(PerformedStep step)
        {
            // because we are dealing with ISet, no need to worry about duplicate entries, since the set doesn't allow it
            _performedSteps.Add(step);
            step.Activities.Add(this);
        }

        /// <summary>
        /// Removes the specified performed step from the set of <see cref="PerformedStep"/> objects associated with this activity,
        /// and removes this activity from the set of activities associated with the specified performed step.
        /// </summary>
        /// <param name="step"></param>
        public virtual void RemovePerformedStep(PerformedStep step)
        {
            if (_performedSteps.Contains(step))
            {
                _performedSteps.Remove(step);
            }

            if (step.Activities.Contains(this))
            {
                step.Activities.Remove(this);
            }
        }

        protected virtual void OnSchedulingChanged()
        {
        }
    }
}
