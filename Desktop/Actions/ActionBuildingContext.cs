using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionBuildingContext"/>.
    /// </summary>
    public class ActionBuildingContext : IActionBuildingContext
    {
        private string _actionID;
        private object _actionTarget;
        private Action _action;
        private ResourceResolver _resolver;

        public ActionBuildingContext(string actionID, object actionTarget)
        {
            _actionID = actionID;
            _actionTarget = actionTarget;

            _resolver = new ActionResourceResolver(_actionTarget);
        }

        public string ActionID
        {
            get { return _actionID; }
        }

        public Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public IResourceResolver ResourceResolver
        {
            get { return _resolver; }
        }

        public object ActionTarget
        {
            get { return _actionTarget; }
        }
    }
}
