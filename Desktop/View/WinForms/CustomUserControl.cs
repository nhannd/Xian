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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// This class should be used the base class for all user controls, rather than directly inheriting
    /// from <see cref="UserControl"/>.  Provides a mechanism for handling default Accept and Cancel buttons.
    /// </summary>
    public class CustomUserControl : UserControl
    {
        private IButtonControl _acceptButton;
        private IButtonControl _cancelButton;

        /// <summary>
        /// Gets or sets the button that will be clicked when the Enter key is pressed
        /// </summary>
        public IButtonControl AcceptButton
        {
            get { return _acceptButton; }
            set { _acceptButton = value; }
        }

        /// <summary>
        /// Gets or sets the button that will be clicked when the Escape key is pressed
        /// </summary>
        public IButtonControl CancelButton
        {
            get { return _cancelButton; }
            set { _cancelButton = value; }
        }

        /// <summary>
        /// Overridden in order to subscribe to the <see cref="Control.Enter"/> event
        /// on all child controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            foreach (Control child in this.Controls)
            {
                child.Enter += ChildEnterEventHandler;
            }

            base.OnEnter(e);
        }

        /// <summary>
        /// Overridden in order to unsubscribe from the <see cref="Control.Enter"/> event
        /// on all child controls and hide the default button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            foreach (Control child in this.Controls)
            {
                child.Enter -= ChildEnterEventHandler;
            }

            // hide the default button
            ShowDefaultButtonUICue(false);

            base.OnLeave(e);
        }

        /// <summary>
        /// Overridden to process Enter and Escape keys
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
			if (base.ProcessDialogKey(keyData))
				return true;

			//if none of the other controls handled it using default processing, 
			// then try our Accept and Cancel buttons, if they are assigned.
			if (keyData == Keys.Return && _acceptButton != null)
			{
					_acceptButton.PerformClick();
					return true;    // handled
			}
			else if (keyData == Keys.Escape && _cancelButton != null)
			{
					_cancelButton.PerformClick();
					return true;    // handled
			}

			return false;
        }

        /// <summary>
        /// Whenever the focused child control changes, the default button UI cues
        /// may need to be updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildEnterEventHandler(object sender, EventArgs e)
        {
            // show the default button iff the active control is not a button
            bool show = !(this.ActiveControl is IButtonControl);
            ShowDefaultButtonUICue(show);
        }

        /// <summary>
        /// Shows the "accept" button with a default button UI cue, if true
        /// </summary>
        /// <param name="show">True to show the UI cue, false to hide it</param>
        private void ShowDefaultButtonUICue(bool show)
        {
            if (_acceptButton != null)
            {
                _acceptButton.NotifyDefault(show);
            }
        }
    }
}
