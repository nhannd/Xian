using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Base class for a workflow performed step.  A performed step records part or all of the performance of
    /// one or more workflow activities (i.e. the relationship between activities and performed steps is many-to-many).
    /// The use of performed steps is entirely optional.  It is perfectly possible to use the Activity model
    /// without recording performed steps.
    /// Note: this class has been coded for compatability with NHibernate mapping.
    /// </summary>
    public abstract class PerformedStep : PersistentFsm<PerformedStepStatus>
    {
        private ISet _activities;
        private ActivityPerformer _performer;
        private DateTime _startTime;
        private DateTime? _endTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedStep()
            :this(null, new PerformedStepStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Constructor that allows the performer to be set
        /// </summary>
        /// <param name="performer"></param>
        public PerformedStep(ActivityPerformer performer)
            :this(performer, new PerformedStepStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="transitionLogic"></param>
        protected PerformedStep(ActivityPerformer performer, IFsmTransitionLogic<PerformedStepStatus> transitionLogic)
            : base(PerformedStepStatus.IP, transitionLogic)
        {
            _activities = new HybridSet();
            _startTime = Platform.Time;
            _performer = performer;
        }


        /// <summary>
        /// Gets the set of associated activities.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="Activity.AddPerformedStep"/> and <see cref="Activity.RemovePerformedStep"/> methods.
        /// </summary>
        public virtual ISet Activities
        {
            get { return _activities; }
        }

        /// <summary>
        /// Gets or sets the performer of this step
        /// </summary>
        public virtual ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        /// <summary>
        /// Gets the start time of this step.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime StartTime
        {
            get { return _startTime; }
            private set { _startTime = value; }
        }

        /// <summary>
        /// Gets the end time of this step.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime? EndTime
        {
            get { return _endTime; }
            private set { _endTime = value; }
        }

        /// <summary>
        /// Discontinues this step
        /// </summary>
        public virtual void Discontinue()
        {
            ChangeState(PerformedStepStatus.DC);
            _endTime = Platform.Time;
        }

        /// <summary>
        /// Completes this step
        /// </summary>
        public virtual void Complete()
        {
            ChangeState(PerformedStepStatus.CM);
            _endTime = Platform.Time;
        }
    }
}
