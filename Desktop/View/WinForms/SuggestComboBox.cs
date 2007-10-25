#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.View.WinForms
{
    public class SuggestComboBox : ComboBox
    {
        private SuggestionProvider _suggestionProvider;
        private bool _strictTyping = true;
        private bool _startWithNullSelection = true;

        private bool _textDeleted;

        #region Public Properties

        /// <summary>
        /// Sets the delegate that takes the query string and return a list of suggestions
        /// </summary>
        public SuggestionProvider SuggestionProvider
        {
            get { return _suggestionProvider; }
            set { _suggestionProvider = value; }
        }

        /// <summary>
        /// Gets or sets the option for strict typing.  StrictTyping restrict input to match text in the suggestions, if the suggestions exist.
        /// </summary>
        [DefaultValue(true)]
        public bool StrictTyping
        {
            get { return _strictTyping; }
            set { _strictTyping = value; }
        }

        /// <summary>
        /// By deafult, the control will start with the first item in the data source.  
        /// This property gets or sets the the option to start the control with null selection.
        /// </summary>
        [DefaultValue(true)]
        public bool StartWithNullSelection
        {
            get { return _startWithNullSelection; }
            set { _startWithNullSelection = value; }
        }

        /// <summary>
        /// Gets or sets currently selected item in the System.Windows.Forms.ComboBox.
        /// If the data source is empty, initialize the data source with the set value
        /// </summary>
        public new object SelectedItem
        {
            get { return base.SelectedItem; }
            set
            {
                if (this.Items.Count == 0 && value != null)
                {
                    ArrayList newArray = new ArrayList();
                    newArray.Add(value);
                    this.DataSource = newArray;
                }

                base.SelectedItem = value;
            }
        }

        #endregion

        #region Override Methods

        protected override void OnCreateControl()
        {
            if (_startWithNullSelection)
                this.SelectedItem = null;

            base.OnCreateControl();
        }

        protected override void OnLeave(EventArgs e)
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
                // doesn't match any suggestions, so clear it
                this.Text = null;
            }

            base.OnLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                case Keys.Back:
                    _textDeleted = true;
                    break;
                default:
                    _textDeleted = false;
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_strictTyping && this.Items.Count > 1 && !Char.IsControl(e.KeyChar))
            {
                // Only check the new char if it's printable (not control char)
                // if there is currently no suggestions, any character is typeable

                string newText = this.Text + e.KeyChar;

                bool newTextIsInSuggestion = false;
                foreach (object item in this.Items)
                {
                    string itemText = GetItemText(item);
                    if (itemText.StartsWith(newText))
                    {
                        newTextIsInSuggestion = true;
                        break;
                    }
                }

                // the new text is not part of any suggestion, 'handled' the character so that this.Text will not be updated
                if (!newTextIsInSuggestion)
                    e.Handled = true;
            }

            base.OnKeyPress(e);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            // Remember the current text as it exist
            string curText = this.Text;
            int cursorPosition = this.SelectionStart;

            // Get new suggestions
            // Setting the datasource will automatically change this.Text
            IList suggestions = _suggestionProvider(curText);
            this.DataSource = suggestions;

            if (this.Items.Count == 0)
            {
                // No suggestion, reset text back to original text
                // and return the cursor to the original position
                this.Text = curText;
                this.SelectionStart = cursorPosition;
                this.SelectionLength = 0;
            }
            else if (this.Items.Count == 1)
            {
                if (_textDeleted)
                {
                    // Special case when user deleted some text but the remaining text matches 
                    // exactly 1 suggestion.  In this case, we allow those text to be deleted,
                    // but shows the dropdown to indicate there is a match suggestion
                    if (!this.DroppedDown)
                        this.DroppedDown = true;

                    this.Text = curText;
                    this.SelectionStart = curText.Length;
                    this.SelectionLength = 0;
                }
                else
                {
                    // Close the dropdown
                    if (this.DroppedDown)
                        this.DroppedDown = false;

                    string itemText = this.GetItemText(this.Items[0]);

                    if (curText == itemText)
                    {
                        // current text is already the item text, no text to append
                        // set cursor to the end
                        this.SelectionStart = itemText.Length;
                        this.SelectionLength = 0;
                    }
                    else
                    {
                        // set current text to item text and highlight the the appended text
                        this.Text = itemText;
                        string appendText = this.Text.Substring(curText.Length);
                        this.SelectionStart = curText.Length;
                        this.SelectionLength = appendText.Length;
                    }
                }
            }
            else // more than 1 suggestion
            {
                // open the dropdown menu
                if (!this.DroppedDown)
                    this.DroppedDown = true;

                // set cursor to the end
                this.Text = curText;
                this.SelectionStart = curText.Length;
                this.SelectionLength = 0;
            }

            base.OnTextUpdate(e);
        }

        #endregion

    }
}
