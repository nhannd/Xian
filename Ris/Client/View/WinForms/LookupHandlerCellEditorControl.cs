using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class LookupHandlerCellEditorControl : UserControl
	{
		private LookupHandlerCellEditor _editor;

		public LookupHandlerCellEditorControl()
		{
			InitializeComponent();
			_suggestBox.ValueChanged += OnSuggestBoxOnValueChanged;
		}

		/// <summary>
		/// Sets the editor on which this control is operating - the control is re-used by the <see cref="TableView"/>.
		/// </summary>
		/// <param name="editor"></param>
		public void SetEditor(LookupHandlerCellEditor editor)
		{
			_editor = editor;

			// change sugg provider
			_suggestBox.SuggestionProvider = _editor.LookupHandler.SuggestionProvider;

			// update value
			_suggestBox.Value = _editor.Value;
		}

		private void _findButton_Click(object sender, EventArgs e)
		{
			try
			{
				object result;
				var resolved = _editor.LookupHandler.Resolve(_suggestBox.Text, true, out result);
				if (resolved)
				{
					_suggestBox.Value = result;
				}
			}
			catch (Exception ex)
			{
				// not much we can do here if Resolve throws an exception
				Platform.Log(LogLevel.Error, ex);
			}
		}

		private void OnSuggestBoxOnValueChanged(object sender, EventArgs e)
		{
			_editor.Value = _suggestBox.Value;
		}

		private void _suggestBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _editor.LookupHandler.FormatItem(e.ListItem);
		}

		private void LookupHandlerCellEditorControl_Load(object sender, EventArgs e)
		{
			_suggestBox.Value = _editor.Value;
		}

	}
}
