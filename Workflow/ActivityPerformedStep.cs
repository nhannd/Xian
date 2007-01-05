using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

namespace ClearCanvas.Workflow
{
    public abstract class ActivityPerformedStep
    {
        private ISet _activities;
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private ActivityPerformedStepStatus _status;

        public ActivityPerformedStep()
        {
            _activities = new HybridSet();
            _status = ActivityPerformedStepStatus.IP;
        }


        /// <summary>
        /// Gets the set of associated activities.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="Activity.AddPerformedStep"/> and <see cref="Activity.RemovePerformedStep"/> methods.
        /// </summary>
        public virtual ISet Activities
        {
            get { return _activities; }
        }

        public virtual ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public virtual DateTime? StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public virtual DateTime? EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public virtual ActivityPerformedStepStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}
