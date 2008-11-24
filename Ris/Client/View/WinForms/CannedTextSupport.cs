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
