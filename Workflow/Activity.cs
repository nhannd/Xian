using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public abstract class Activity : Entity
    {
        private ISet _performedSteps;
        private ActivityScheduling _scheduling;
        private ActivityStatus _status;

        public Activity()
        {
            _performedSteps = new HybridSet();
            _scheduling = new ActivityScheduling();
            _status = ActivityStatus.SC;
        }

        /// <summary>
        /// Gets the set of associated performed steps.  Do not add or removes elements directly from this collection.
        /// Instead use the <see cref="AddPerformedStep"/> and <see cref="RemovePerformedStep"/> methods.
        /// </summary>
        public virtual ISet PerformedSteps
        {
            get { return _performedSteps; }
        }

        public virtual ActivityScheduling Scheduling
        {
            get { return _scheduling; }
            set { _scheduling = value; }
        }

        public virtual ActivityStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public virtual bool IsTerminated
        {
            get { return _status == ActivityStatus.CM || _status == ActivityStatus.DC; }
        }

        public virtual void AddPerformedStep(ActivityPerformedStep step)
        {
            // because we are dealing with ISet, no need to worry about duplicate entries, since the set doesn't allow it
            _performedSteps.Add(step);
            step.Activities.Add(this);
        }

        public virtual void RemovePerformedStep(ActivityPerformedStep step)
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
    }
}
