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
