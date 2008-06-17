using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using System.Drawing;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Adds canned-text in-place lookup support to a rich text box.
	/// </summary>
    public class CannedTextSupport : IDisposable
    {
        private RichTextBox _textEditor;
        private ICannedTextLookupHandler _lookupHandler;
        private PopupForm _popup;
        private CannedTextInplaceLookupControl _lookup;

        public CannedTextSupport(RichTextBox textEditor, ICannedTextLookupHandler lookupHandler)
        {
            _textEditor = textEditor;
            _lookupHandler = lookupHandler;

            Initialize();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _textEditor.KeyDown -= _textEditor_KeyDown;
        }

        #endregion

        private void Initialize()
        {
            _textEditor.KeyDown += _textEditor_KeyDown;
        }

        private void _textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.OemPeriod)
            {
                Point pt = _textEditor.GetPositionFromCharIndex(_textEditor.SelectionStart);

                _lookup = new CannedTextInplaceLookupControl(_lookupHandler);
                _lookup.Cancelled += new EventHandler(_lookup_Cancelled);
                _lookup.Committed += new EventHandler(_lookup_Committed);

                _popup = new PopupForm(_lookup, _textEditor, _textEditor.PointToScreen(pt));
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
					_textEditor.SelectedText = _lookupHandler.GetFullText(cannedText);
				else
	                _textEditor.SelectedText = cannedText.Text;
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
