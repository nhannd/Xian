#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Reflection;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Special combo-box that works with a <see cref="ISuggestionProvider"/> to populate the drop-down list
    /// with suggested items as the user types.
    /// </summary>
    public class SuggestComboBox : ComboBox
    {
        private ISuggestionProvider _suggestionProvider;

        private event EventHandler _valueChanged;
        #region Public properties

        /// <summary>
        /// Gets or sets the <see cref="ISuggestionProvider"/>.
        /// </summary>
        [Browsable(false)]
        public ISuggestionProvider SuggestionProvider
        {
            get { return _suggestionProvider; }
            set
            {
                if (_suggestionProvider != null)
                    _suggestionProvider.SuggestionsProvided -= ItemsProvidedEventHandler;

                _suggestionProvider = value;

                if (_suggestionProvider != null)
                {
                    _suggestionProvider.SuggestionsProvided += ItemsProvidedEventHandler;

					// Bug #2222: only call SetQuery if this is a "simple" dropdown, in which case
					// we need to populate the list immediately
					// calling it for other styles causes behaviour described in #2222,
					// and there is really no need to get suggestions until user actually types
					if (this.DropDownStyle == ComboBoxStyle.Simple)
						_suggestionProvider.SetQuery(this.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current value of the control.
        /// </summary>
        [Browsable(false)]
        public object Value
        {
            get { return this.SelectedItem; }
            set
            {
                if(!Equals(this.SelectedItem, value))
                {
                    // in order to set the value, the Items collection must contain the value
                    // but if the value is null, can just use an empty list
                    // (also need to check for DBNull, for some stupid reason)
                    ArrayList items = new ArrayList();
                    if(value != null && value != System.DBNull.Value)
                    {
                        items.Add(value);
                    }
                    else
                    {
                        // if value is null or DBNull, clear the text
                        this.Text = null;
                    }

                    UpdateListItems(items);
                    this.SelectedItem = value;

                    OnValueChanged(EventArgs.Empty);
                }

            }
        }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property changes.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        #endregion

        #region Overrides and Helpers

        protected virtual void OnValueChanged(EventArgs args)
        {
            EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
        }

        protected override void OnCreateControl()
        {
            this.SelectedItem = null;

            base.OnCreateControl();
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            // there are 2 ways that the value can change
            // either the selection change is comitted, or the control loses focus
            OnValueChanged(EventArgs.Empty);

            base.OnSelectionChangeCommitted(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            try
            {
                // do a case-insensitive search
                int itemIndex = this.FindStringExact(this.Text);
                if (itemIndex > -1)
                {
                    // update the selected index
                    this.SelectedIndex = itemIndex;

                    // also update the visible text, because the upper/lower-casing may not match
                    object item = this.Items[itemIndex];
                    this.Text = GetItemText(item);
                }
                else
                {
                    // doesn't match any suggestions, clear the text
					this.Text = null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // if the combo box is dropped down, and the control loses focus (calling OnLeave),
                // it seems WinForms throws an exception from this.Text
                // not sure why this happens, but there is really nothing that can be done in terms of recovery
            }

            // there are 2 ways that the value can change
            // either the selection change is comitted, or the control loses focus
            OnValueChanged(EventArgs.Empty);

            base.OnLeave(e);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
				case Keys.Escape:
					this.Text = null;
            		OnSelectionChangeCommitted(e);
            		break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);
			CursorReset();
            _suggestionProvider.SetQuery(this.Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // setting this to null will also unsubscribe from the last subscribed-to provider
                this.SuggestionProvider = null;
            }

            base.Dispose(disposing);
        }

        private void ItemsProvidedEventHandler(object sender, SuggestionsProvidedEventArgs e)
        {
            // Remember the current text and selection start,
            // as they exists prior to modifying the items collection
            string curText = this.Text;
            int cursorPosition = this.SelectionStart;

            if (e.Items.Count == 0)
            {
				try
				{
					this.DroppedDown = false;
					// there are no suggestions, so clear the items list
					this.Items.Clear();
					// reset text back to original text
					// and return the cursor to the original position
					this.Text = curText;
					this.SelectionStart = cursorPosition;
				}
				catch (ArgumentOutOfRangeException)
				{
					// just in case...it throws the exception while clearing the list
				}
            }
            else
            {
                // at least 1 suggestion exists

                // open the dropdown menu
                this.DroppedDown = true;

                // update list
                UpdateListItems(e.Items);

                // set cursor to the end
                this.Text = curText;
                this.SelectionStart = curText.Length;
                this.SelectionLength = 0;
            }
        }

        private void UpdateListItems(IList items)
        {
			this.Items.Clear();
			if (items.Count > 0)
			{
				object[] array = new object[items.Count];
				items.CopyTo(array, 0);
				this.Items.AddRange(array);
			}
        }

        #endregion

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// SuggestComboBox
			// 
			this.CursorChanged += new System.EventHandler(this.SuggestComboBox_CursorChanged);
			this.ResumeLayout(false);
		}

		private void CursorReset()
		{
			Cursor.Current = Cursors.Default;
			Cursor.Show();
		}

		private void SuggestComboBox_CursorChanged(object sender, EventArgs e)
		{
			CursorReset();
		}
    }
}
