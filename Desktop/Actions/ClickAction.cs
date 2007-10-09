using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Security;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IClickAction"/>.  Models a user-interface action that is invoked by
    /// a click, such as a toolbar button or a menu item.
    /// </summary>
    public class ClickAction : Action, IClickAction
    {
		private readonly ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;
		private XKeys _keyStroke;

        private bool _checked = false;
        private event EventHandler _checkedChanged;

		private bool _checkParents = false;

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
            _checked = false;
        }

        /// <summary>
        /// Sets the delegate that will respond when this action is clicked.
        /// </summary>
        /// <param name="clickHandler"></param>
        public void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }

        /// <summary>
        /// Sets the key stroke that can be used to invoke this action from the keyboard.
        /// </summary>
        /// <param name="keyStroke"></param>
		public void SetKeyStroke(XKeys keyStroke)
		{
			_keyStroke = keyStroke;
		}

        #region IClickAction members

        /// <summary>
        /// Gets a value indicating whether this action is a "check" action, that is, an action that behaves as a toggle.
        /// </summary>
        public bool IsCheckAction
        {
            get { return (_flags & ClickActionFlags.CheckAction) == 0 ? false : true; }
        }

        /// <summary>
        /// Gets the checked state that the action should present in the UI, if this is a "check" action.
        /// </summary>
        /// <remarks>
        /// This property has no meaning if <see cref="IClickAction.IsCheckAction"/> returns false.
        /// </remarks>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IClickAction.Checked"/> property of this action changes.
        /// </summary>
        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }

        /// <summary>
        /// Gets a value indicating whether parent items should be checked if this
        /// <see cref="IClickAction"/> is checked.
        /// </summary>
        public bool CheckParents
		{
			get { return _checkParents; }
			set { _checkParents = value; }
		}

        /// <summary>
        /// Called by the UI when the user clicks on the action.
        /// </summary>
        public void Click()
        {
            if (_clickHandler != null)
            {
                _clickHandler();
            }
        }

        /// <summary>
        /// Gets the keystroke that the UI should attempt to intercept to invoke the action.
        /// </summary>
        public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

        #endregion
	}
}
