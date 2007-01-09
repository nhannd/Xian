using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public class ActivityStatusTransitionMatrix
    {
        private static readonly bool[,] _transitions = new bool[,] {
            // to: SC,   IP,   SU,   CM,   DC           // from:
                { false, true, true, true, true },      // SC
                { false, false, true, true, true },     // IP
                { false, true, false, true, true },     // SU
                { false, false, false, false, false},   // CM
                { false, false, false, false, false},   // DC
            };

        public virtual bool IsAllowed(ActivityStatus from, ActivityStatus to)
        {
            return _transitions[(int)from, (int)to];
        }
    }
}
