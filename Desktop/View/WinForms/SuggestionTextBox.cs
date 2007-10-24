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
using System.Windows.Forms;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// A delegate that takes the query string and return a list of suggestions
    /// </summary>
    /// <param name="query"></param>
    /// <returns>A list of suggestions</returns>
    public delegate object[] SuggestDelegate(string query);

    /// <summary>
    /// A delegate that takes an object and return the formatted string
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>The formatted string</returns>
    public delegate string FormatDelegate(object obj);

    public class SuggestionTextBox : TextBox
    {
        private readonly List<object> _suggestions;
        private FormatDelegate _formatDelegate;
        private SuggestDelegate _suggestDelegate;
        private string _lastQueryWithSuggestions;
        private string _previousText;
        private bool _strictTyping;

        public SuggestionTextBox()
        {
            _suggestions = new List<object>();

            this.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.TextChanged += SuggestionTextBox_TextChanged;
            this.KeyPress += SuggestionTextBox_KeyPress;
        }

        #region Events Handlers

        void SuggestionTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_strictTyping && _suggestions.Count > 0 && !Char.IsControl(e.KeyChar))
            {
                // Only check the new char if it's printable (not control char)
                // if there is currently no suggestions, any character is typeable

                string newText = this.Text + e.KeyChar;

                bool newTextIsInSuggestion = CollectionUtils.Contains<string>(this.AutoCompleteCustomSource,
                    delegate(string suggestion)
                    {
                        return suggestion.StartsWith(newText);
                    });

                if (!newTextIsInSuggestion)
                {
                    // the new text is not part of any suggestion, 'handled' the character so that this.Text will not be updated
                    e.Handled = true;
                    return;
                }
            }

            _previousText = this.Text;
        }

        void SuggestionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suggestDelegate == null)
                return;

            // if the current text starts with the last query string with suggestion, there is no need to re-query
            // because the current suggestion list is still valid
            if (!String.IsNullOrEmpty(_lastQueryWithSuggestions) && this.Text.StartsWith(_lastQueryWithSuggestions))
                return;

            // backspace or some text is removed from the end
            if (IsTextRemoved)
            {
                ClearSuggestions();
                return;
            }

            ResetSuggestions(this.Text, _suggestDelegate(this.Text));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the delegate that takes the query string and return a list of suggestions
        /// </summary>
        public SuggestDelegate SuggestionDelegate
        {
            set
            {
                if (_suggestDelegate != value)
                    _suggestDelegate = value;
            }
        }

        /// <summary>
        /// Sets the delegate that takes an object and return the formatted string.  Either implement this delegate or the object.ToString() method
        /// </summary>
        public FormatDelegate FormatDelegate
        {
            set
            {
                if (_formatDelegate != value)
                    _formatDelegate = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected object
        /// </summary>
        public object SelectedSuggestion
        {
            get
            {
                if (String.IsNullOrEmpty(this.Text) || _suggestions.Count == 0)
                    return null;

                return CollectionUtils.SelectFirst<object>(_suggestions,
                    delegate(object obj)
                    {
                        return Equals(this.Text, FormatObject(obj));
                    });
            }
            set
            {
                if (value == null || _suggestDelegate == null)
                {
                    this.Text = "";
                    return;
                }

                // If value does not exist, add to suggestions
                // This often happens when the textbox is initialized with a value
                if (_suggestions.Count == 0 || !_suggestions.Contains(value))
                {
                    List<object> newCollection = new List<object>();
                    newCollection.AddRange(_suggestions);
                    newCollection.Add(value);
                    ResetSuggestions(this.Text, newCollection.ToArray());
                }

                this.Text = FormatObject(value);
            }
        }

        /// <summary>
        /// Gets or sets the strict typing.  StrictTyping restrict input to match text in the suggestions, if the suggestions exist.
        /// </summary>
        public bool StrictTyping
        {
            get { return _strictTyping; }
            set { _strictTyping = value; }
        }

        #endregion

        #region Private Helpers

        private bool IsTextRemoved
        {
            get
            {
                if (String.IsNullOrEmpty(this.Text))
                    return true;

                if (String.IsNullOrEmpty(_previousText))
                    return false;

                if (this.Text.Length < _previousText.Length)
                    return true;

                return false;
            }
        }

        private string FormatObject(object obj)
        {
            if (_formatDelegate == null)
                return obj.ToString();

            return _formatDelegate(obj);
        }

        private void ClearSuggestions()
        {
            ResetSuggestions(null, null);
        }

        private void ResetSuggestions(string query, object[] suggestions)
        {
            // Need to set the AutoCompleteMode to none first
            // Otherwise the control will crash while the AutoCompleteCustomSource is modified
            AutoCompleteMode originalMode = this.AutoCompleteMode;
            this.AutoCompleteMode = AutoCompleteMode.None;

            _suggestions.Clear();
            this.AutoCompleteCustomSource.Clear();

            if (suggestions == null || suggestions.Length == 0)
            {
                _lastQueryWithSuggestions = null;
            }
            else
            {
                _lastQueryWithSuggestions = query;

                _suggestions.AddRange(suggestions);

                List<string> suggestionString = CollectionUtils.Map<object, string, List<string>>(suggestions,
                    delegate(object obj)
                    {
                        return FormatObject(obj);
                    });

                this.AutoCompleteCustomSource.AddRange(suggestionString.ToArray());
            }

            // Set the AutoCompleteMode back
            this.AutoCompleteMode = originalMode;
        }

        #endregion
    }
}
