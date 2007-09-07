using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Base class for <see cref="Activity"/> and <see cref="PerformedStep"/>.
    /// Note: this class has been coded for compatability with NHibernate mapping.
    /// </summary>
    /// <typeparam name="TStateEnum"></typeparam>
    public class PersistentFsm<TStateEnum> : Entity
    {
        private TStateEnum _state;
        private DateTime _lastStateChangeTime;
        private DateTime _creationTime;
        private IFsmTransitionLogic<TStateEnum> _transitionLogic;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="transitionLogic"></param>
        public PersistentFsm(TStateEnum initialState, IFsmTransitionLogic<TStateEnum> transitionLogic)
        {
            _transitionLogic = transitionLogic;
            _creationTime = _lastStateChangeTime = Platform.Time;
            _state = initialState;
        }

        /// <summary>
        /// Subclasses must use this method to change the state of this object.  This method validates
        /// the legality of the state transition before allowing the state change.
        /// </summary>
        /// <param name="state"></param>
        protected void ChangeState(TStateEnum state)
        {
            if (_transitionLogic != null)
            {
                if (!_transitionLogic.IsAllowed(_state, state))
                    throw new IllegalStateTransitionException(string.Format(SR.ExceptionIllegalStateTransition, _state, state));
            }

            TStateEnum previousState = _state;
            _state = state;
            _lastStateChangeTime = Platform.Time;

            OnStateChanged(previousState, _state);
        }

        /// <summary>
        /// Gets the current state.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual TStateEnum State
        {
            get { return _state; }
            private set { _state = value; }
        }

        /// <summary>
        /// Gets the creation time of this object.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime CreationTime
        {
            get { return _creationTime; }
            private set { _creationTime = value; }
        }

        /// <summary>
        /// Get the time of the last state change for this object.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime LastStateChangeTime
        {
            get { return _lastStateChangeTime; }
            private set { _lastStateChangeTime = value; }
        }

        /// <summary>
        /// Gets the transition law that determines the legal state transitions
        /// </summary>
        public virtual IFsmTransitionLogic<TStateEnum> TransitionLaw
        {
            get { return _transitionLogic; }
        }

        /// <summary>
        /// True if this object is in its initial state.
        /// </summary>
        public virtual bool IsInitial
        {
            get { return _transitionLogic.IsInitial(_state); }
        }

        /// <summary>
        /// True if this object is in a terminal state.
        /// </summary>
        public virtual bool IsTerminated
        {
            get { return _transitionLogic.IsTerminal(_state); }
        }

        /// <summary>
        /// Called when the state changes to allow subclasses to respond.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="newState"></param>
        protected virtual void OnStateChanged(TStateEnum previousState, TStateEnum newState)
        {
        }
    }
}
