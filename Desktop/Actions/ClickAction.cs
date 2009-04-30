#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Used by <see cref="ClickAction"/> objects to establish a handler for a click.
	/// </summary>
	public delegate void ClickHandlerDelegate();
	
	/// <summary>
    /// Default implementation of <see cref="IClickAction"/> which models a user-interface action that is invoked by
    /// a click, such as a toolbar button or a menu item.
    /// </summary>
    public class ClickAction : Action, IClickAction
    {
		private readonly ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;
		private XKeys _keyStroke;

        private bool _checked = false;
        private event EventHandler _checkedChanged;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that control the style of the action.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources.</param>
        public ClickAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
        {
            _flags = flags;
            _checked = false;
        }

        /// <summary>
        /// Sets the delegate that will respond when this action is clicked.
        /// </summary>
        public void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }

        #region IClickAction members

		/// <summary>
		/// Gets the keystroke that the UI should attempt to intercept to invoke the action.
		/// </summary>
		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}
		
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
			get { return (_flags & ClickActionFlags.CheckParents) == ClickActionFlags.CheckParents; }
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

        #endregion
	}
}
