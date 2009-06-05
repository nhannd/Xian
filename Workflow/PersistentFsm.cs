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
        /// This property allows protected set for NHibernate mapping compatability.
        /// </summary>
        public virtual TStateEnum State
        {
            get { return _state; }
            protected set { _state = value; }
        }

        /// <summary>
        /// Gets the creation time of this object.
        /// This property allows protected set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime CreationTime
        {
            get { return _creationTime; }
            protected set { _creationTime = value; }
        }

        /// <summary>
        /// Get the time of the last state change for this object.
        /// This property allows protected set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime LastStateChangeTime
        {
            get { return _lastStateChangeTime; }
            protected set { _lastStateChangeTime = value; }
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
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_lastStateChangeTime = _lastStateChangeTime.AddMinutes(minutes);
			_creationTime = _creationTime.AddMinutes(minutes);
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
