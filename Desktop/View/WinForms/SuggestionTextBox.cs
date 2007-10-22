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

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// A delegate takes the query string and return a list of suggestions
    /// </summary>
    /// <param name="query"></param>
    /// <returns>A list of suggestions</returns>
    public delegate string[] SuggestDelegate(string query);

    public class SuggestionTextBox : TextBox
    {
        private SuggestDelegate _suggestDelegate;
        private string _lastQueryWithSuggestions;
        private string _previousText;

        public SuggestionTextBox()
        {
            this.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.TextChanged += SuggestionTextBox_TextChanged;
            this.KeyPress += SuggestionTextBox_KeyPress;
        }

        void SuggestionTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            _previousText = this.Text;
        }

        void SuggestionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suggestDelegate == null)
                return;

            // backspace or some text is removed from the end
            if (IsTextRemoved)
            {
                if (!String.IsNullOrEmpty(_lastQueryWithSuggestions) && this.Text.StartsWith(_lastQueryWithSuggestions))
                    return;

                ClearSuggestions();
                return;
            }

            // if the current text starts with the last query string with suggestion, there is no need to re-query
            // because the current suggestion list is still valid
            if (!String.IsNullOrEmpty(_lastQueryWithSuggestions) && this.Text.StartsWith(_lastQueryWithSuggestions))
                return;

            string[] suggestions = _suggestDelegate(this.Text);
            ResetSuggestions(this.Text, suggestions);
        }

        public SuggestDelegate SuggestionDelegate
        {
            set
            {
                if (_suggestDelegate != value)
                    _suggestDelegate = value;
            }
        }

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

        private void ClearSuggestions()
        {
            ResetSuggestions(null, null);
        }

        private void ResetSuggestions(string query, string[] suggestions)
        {
            // Need to set the AutoCompleteMode to none first
            // Otherwise the control will crash while the AutoCompleteCustomSource is modified
            AutoCompleteMode originalMode = this.AutoCompleteMode;
            this.AutoCompleteMode = AutoCompleteMode.None;

            this.AutoCompleteCustomSource.Clear();

            if (suggestions == null || suggestions.Length == 0)
            {
                _lastQueryWithSuggestions = null;
            }
            else
            {
                _lastQueryWithSuggestions = query;
                this.AutoCompleteCustomSource.AddRange(suggestions);
            }

            // Set the AutoCompleteMode back
            this.AutoCompleteMode = originalMode;
        }
    }
}
