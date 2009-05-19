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
using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using Iesi.Collections.Generic;

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
        private ISet<Activity> _activities;
        private ActivityPerformer _performer;
        private DateTime _startTime;
        private DateTime? _endTime;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedStep()
            :this(null, null, new PerformedStepStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Constructor that allows the performer to be set
        /// </summary>
        /// <param name="performer"></param>
        public PerformedStep(ActivityPerformer performer)
            :this(performer, null, new PerformedStepStatusTransitionLogic())
        {
        }

		/// <summary>
		/// Constructor that allows the performer to be set, and the start-time to be specified.
		/// </summary>
		/// <param name="performer"></param>
		/// <param name="startTime"></param>
		public PerformedStep(ActivityPerformer performer, DateTime? startTime)
			: this(performer, startTime, new PerformedStepStatusTransitionLogic())
		{
		}

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="startTime"></param>
        /// <param name="transitionLogic"></param>
        protected PerformedStep(ActivityPerformer performer, DateTime? startTime, IFsmTransitionLogic<PerformedStepStatus> transitionLogic)
            : base(PerformedStepStatus.IP, transitionLogic)
        {
            _activities = new HashedSet<Activity>();
			_startTime = startTime ?? Platform.Time;
            _performer = performer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the set of associated activities.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="Activity.AddPerformedStep"/> and <see cref="Activity.RemovePerformedStep"/> methods.
        /// </summary>
        public virtual ISet<Activity> Activities
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

        #endregion

        /// <summary>
        /// Discontinues this step
        /// </summary>
        public virtual void Discontinue()
        {
			Discontinue((DateTime?)null);
        }

		/// <summary>
		/// Discontinues this step
		/// </summary>
		public virtual void Discontinue(DateTime? endTime)
		{
			ChangeState(PerformedStepStatus.DC);
			_endTime = endTime ?? Platform.Time;
		}

		        /// <summary>
        /// Completes this step
        /// </summary>
		public virtual void Complete()
        {
			Complete((DateTime?)null);
        }

    	/// <summary>
        /// Completes this step
        /// </summary>
        public virtual void Complete(DateTime? endTime)
        {
            ChangeState(PerformedStepStatus.CM);
			_endTime = endTime ?? Platform.Time;
		}
    }
}
