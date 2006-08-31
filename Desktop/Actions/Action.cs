using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="IAction"/>.  Action classes should
    /// inherit this class rather than implement <see cref="IAction"/> directly.
    /// </summary>
    public abstract class Action : IAction
    {
        private string _actionID;
        private ActionPath _path;
        private object _target;

        private string _tooltip;
        private IconSet _iconSet;
        private string _label;

        private IObservablePropertyBinding<bool> _enabledPropertyBinding;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID">The logical action ID</param>
        /// <param name="pathHint">The action path</param>
        /// <param name="target"></param>
        public Action(string actionID, ActionPath path, object target)
        {
            _actionID = actionID;
            _path = path;
            _target = target;
        }

        /// <summary>
        /// Sets the observable property that this action monitors for its enablement state
        /// </summary>
        /// <param name="enabledPropertyBinding">The property to monitor</param>
        public void SetEnabledObservable(IObservablePropertyBinding<bool> enabledPropertyBinding)
        {
            _enabledPropertyBinding = enabledPropertyBinding;
        }

        #region IAction members

        public string ActionID
        {
            get { return _actionID; }
        }

        public object Target
        {
            get { return _target; }
        }

        public ActionPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public IconSet IconSet
        {
            get { return _iconSet; }
            set { _iconSet = value; }
        }
        
        public bool Enabled
        {
            get
            {
                return _enabledPropertyBinding == null ? true // smart default
                    : _enabledPropertyBinding.PropertyValue;
            }
        }

        public event EventHandler EnabledChanged
        {
            add
            {
                if (_enabledPropertyBinding != null)
                    _enabledPropertyBinding.PropertyChanged += value;
            }
            remove
            {
                if (_enabledPropertyBinding != null)
                    _enabledPropertyBinding.PropertyChanged -= value;
            }
        }

        #endregion

    }
}
