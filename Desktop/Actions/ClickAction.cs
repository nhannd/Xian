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


		public void SetKeyStroke(XKeys keyStroke)
		{
			_keyStroke = keyStroke;
		}

        #region IClickAction members

        public bool IsCheckAction
        {
            get { return (_flags & ClickActionFlags.CheckAction) == 0 ? false : true; }
        }

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

        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }

		public bool CheckParents
		{
			get { return _checkParents; }
			set { _checkParents = value; }
		}
		
		public void Click()
        {
            if (_clickHandler != null)
            {
                _clickHandler();
            }
        }

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

        #endregion
	}
}
