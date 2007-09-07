using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Defines the basic FSM for allowable <see cref="ActivityPerformedStepStatus"/> transitions.  Subclass this class
    /// and override the <see cref="IsAllowed"/> method to customize the FSM for a particular scenario.
    /// </summary>
    public class PerformedStepStatusTransitionLogic : IFsmTransitionLogic<PerformedStepStatus>
    {
        private static readonly bool[,] _transitions = new bool[,] {
            // to:   IP,    CM,   DC       // from:
                { false, true, true },     // IP
                { false, false, false},   // CM
                { false, false, false},   // DC
            };


        #region IFsmTransitionLogic<ActivityPerformedStepStatus> Members

        /// <summary>
        /// Returns a boolean value indicating whether the specified transition is allowed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual bool IsAllowed(PerformedStepStatus from, PerformedStepStatus to)
        {
            return _transitions[(int)from, (int)to];
        }

        public bool IsTerminal(PerformedStepStatus state)
        {
            return state == PerformedStepStatus.CM || state == PerformedStepStatus.DC;
        }

        public bool IsInitial(PerformedStepStatus state)
        {
            return state == PerformedStepStatus.IP;
        }

        #endregion
    }
}
