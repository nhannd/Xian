using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Base class providing a default implementation of <see cref="IClickAction"/>.  Action classes should
    /// inherit this class rather than implement <see cref="IClickAction"/> directly.
    /// </summary>
    public class ClickAction : Action, IClickAction
    {
        private ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;

        private IObservablePropertyBinding<bool> _checkedPropertyBinding;

        public ClickAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
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

        public void SetCheckedObservable(IObservablePropertyBinding<bool> checkedPropertyBinding)
        {
            _checkedPropertyBinding = checkedPropertyBinding;
        }

        public void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }
    }
}
