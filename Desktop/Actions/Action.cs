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
        private ActionCategory _category;
        private ActionPath _path;
        private object _target;

        private string _tooltip;
        private IconSet _iconSet;
        private string _label;

        private IObservablePropertyBinding<bool> _enabledPropertyBinding;

        public Action(string actionID, ActionCategory category, ActionPath pathHint, object target)
        {
            _actionID = actionID;
            _category = category;
            _path = pathHint;
            _target = target;
        }

        public string ActionID
        {
            get { return _actionID; }
        }

        public object Target
        {
            get { return _target; }
        }

        public ActionCategory Category
        {
            get { return _category; }
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

        internal void SetEnabledObservable(IObservablePropertyBinding<bool> enabledPropertyBinding)
        {
            _enabledPropertyBinding = enabledPropertyBinding;
        }
    }
}
