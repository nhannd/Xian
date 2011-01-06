#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionBuildingContext"/>.
    /// </summary>
    internal class ActionBuildingContext : IActionBuildingContext
    {
        private readonly string _actionID;
        private readonly object _actionTarget;
        private readonly ResourceResolver _resolver;
        private Action _action;

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
