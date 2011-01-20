#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
