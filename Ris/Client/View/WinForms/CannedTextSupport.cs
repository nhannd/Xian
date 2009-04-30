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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Adds canned-text in-place lookup support to a rich text box.
	/// </summary>
    public class CannedTextSupport : IDisposable
    {
		class RichTextBoxOwner : IRichTextBoxOwner
		{
			private readonly RichTextBox _richTextBox;

			public RichTextBoxOwner(RichTextBox richTextBox)
			{
				_richTextBox = richTextBox;
			}

			#region IRichTextBoxOwner Members

			public RichTextBox GetRichTextBox()
			{
				return _richTextBox;
			}

			public void SetSelectedText(string text)
			{
				_richTextBox.SelectedText = text;
			}

			#endregion
		}

		private readonly IRichTextBoxOwner _textEditor;
        private readonly ICannedTextLookupHandler _lookupHandler;
        private PopupForm _popup;
        private CannedTextInplaceLookupControl _lookup;

		public CannedTextSupport(RichTextBox control, ICannedTextLookupHandler lookupHandler)
			: this(new RichTextBoxOwner(control), lookupHandler)
		{
		}

		public CannedTextSupport(IRichTextBoxOwner control, ICannedTextLookupHandler lookupHandler)
        {
			_textEditor = control;
            _lookupHandler = lookupHandler;

            Initialize();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _textEditor.GetRichTextBox().KeyDown -= _textEditor_KeyDown;
        }

        #endregion

        private void Initialize()
        {
			_textEditor.GetRichTextBox().KeyDown += _textEditor_KeyDown;
        }

        private void _textEditor_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.Control && e.KeyCode == Keys.OemPeriod)
            {
				Point pt = _textEditor.GetRichTextBox().GetPositionFromCharIndex(_textEditor.GetRichTextBox().SelectionStart);

                _lookup = new CannedTextInplaceLookupControl(_lookupHandler);
                _lookup.Cancelled += _lookup_Cancelled;
                _lookup.Committed += _lookup_Committed;

				_popup = new PopupForm(_lookup, _textEditor.GetRichTextBox(), _textEditor.GetRichTextBox().PointToScreen(pt));
                _popup.ShowPopup();
            }
        }

        private void _lookup_Committed(object sender, EventArgs e)
        {
            CannedText cannedText = (CannedText)_lookup.Value;
            _popup.ClosePopup();
            _popup = null;
            _lookup = null;

            if (cannedText != null)
            {
				if (cannedText.IsSnippet)
					_textEditor.SetSelectedText(_lookupHandler.GetFullText(cannedText));
				else
					_textEditor.SetSelectedText(cannedText.Text);
            }
        }

        private void _lookup_Cancelled(object sender, EventArgs e)
        {
            _popup.ClosePopup();
            _popup = null;
            _lookup = null;
        }
    }
}
