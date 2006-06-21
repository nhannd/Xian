using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="IClickAction"/>.  Action classes should
    /// inherit this class rather than implement <see cref="IClickAction"/> directly.
    /// </summary>
    public abstract class ClickAction : Action, IClickAction
    {
        private ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;

        private IObservablePropertyBinding<bool> _checkedPropertyBinding;

        public ClickAction(string actionID, ActionCategory category, ActionPath path, object target, ClickActionFlags flags)
            :base(actionID, category, path, target)
        {
            _flags = flags;
        }

        public bool IsCheckAction
        {
            get { return (_flags & ClickActionFlags.CheckAction) == 0 ? false : true; }
        }

        public bool Checked
        {
            get
            {
                return _checkedPropertyBinding == null ? false // smart default
                    : _checkedPropertyBinding.PropertyValue;
            }
        }

        public event EventHandler CheckedChanged
        {
            add
            {
                if (_checkedPropertyBinding != null)
                    _checkedPropertyBinding.PropertyChanged += value;
            }
            remove
            {
                if (_checkedPropertyBinding != null)
                    _checkedPropertyBinding.PropertyChanged -= value;
            }
        }

        public void Click()
        {
            if (_clickHandler != null)
            {
                _clickHandler();
            }
        }

        internal void SetCheckedObservable(IObservablePropertyBinding<bool> checkedPropertyBinding)
        {
            _checkedPropertyBinding = checkedPropertyBinding;
        }

        internal void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }
    }
}
