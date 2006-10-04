using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IClickAction"/>.
    /// </summary>
    public class ClickAction : Action, IClickAction
    {
        private ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;

        private IObservablePropertyBinding<bool> _checkedPropertyBinding;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID">The fully qualified action ID</param>
        /// <param name="path">The action path</param>
        /// <param name="flags">Flags that control the style of the action</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources</param>
        public ClickAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
        {
            _flags = flags;
        }

        /// <summary>
        /// Sets the observable property that this action monitors for its checked state
        /// </summary>
        /// <param name="checkedPropertyBinding">The property to monitor</param>
        public void SetCheckedObservable(IObservablePropertyBinding<bool> checkedPropertyBinding)
        {
            _checkedPropertyBinding = checkedPropertyBinding;
        }

        /// <summary>
        /// Sets the delegate that will respond when this action is clicked.
        /// </summary>
        /// <param name="clickHandler"></param>
        public void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }


        #region IClickAction members

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

        #endregion

    }
}
