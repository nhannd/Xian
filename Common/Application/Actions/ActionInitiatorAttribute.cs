using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare an action.
    /// </summary>
    public abstract class ActionInitiatorAttribute : ActionAttribute
    {
        private ActionCategory _category;
        
        public ActionInitiatorAttribute(string actionID, ActionCategory category)
            : base(actionID)
        {
            _category = category;
        }

        public ActionCategory Category { get { return _category; } }
    }
}
