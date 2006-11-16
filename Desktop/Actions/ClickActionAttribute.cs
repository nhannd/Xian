using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare "click" actions.
    /// </summary>
    public abstract class ClickActionAttribute : ActionInitiatorAttribute
    {
        private string _path;
        private ClickActionFlags _flags;
		private XKeys _keyStroke;

        public ClickActionAttribute(string actionID, string path)
            :base(actionID)
        {
            _path = path;
            _flags = ClickActionFlags.None; // default value, will override if named parameter is specified
        }

        /// <summary>
        /// Flags that customize the behaviour of the action.
        /// </summary>
        public ClickActionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

        /// <summary>
        /// The suggested location of the action in the action model.
        /// </summary>
        public string Path { get { return _path; } }

    }
}
