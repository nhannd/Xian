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

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Defines the basic FSM logic for allowable <see cref="ActivityStatus"/> transitions.  Subclass this class
    /// and override the <see cref="IsAllowed"/> method to customize the logic for a particular scenario.
    /// </summary>
    public class ActivityStatusTransitionLogic : IFsmTransitionLogic<ActivityStatus>
    {
        private static readonly bool[,] _transitions = new bool[,] {
            // to: SC,   IP,   SU,   CM,   DC           // from:
                { false, true, true, true, true },      // SC
                { false, false, true, true, true },     // IP
                { false, true, false, true, true },     // SU
                { false, false, false, false, false},   // CM
                { false, false, false, false, false},   // DC
            };

        /// <summary>
        /// Returns a boolean value indicating whether the specified transition is allowed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual bool IsAllowed(ActivityStatus from, ActivityStatus to)
        {
            return _transitions[(int)from, (int)to];
        }

        public virtual bool IsTerminal(ActivityStatus state)
        {
            return state == ActivityStatus.CM || state == ActivityStatus.DC;
        }

        public bool IsInitial(ActivityStatus state)
        {
            return state == ActivityStatus.SC;
        }
    }
}
